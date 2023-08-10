using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.ChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Events;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Handlers;

public class GetJoinedChartHandler : IRequestHandler<GetJoinedChartRequestDto, Result<GetJoinedCandleResponseDto>>
{
  private readonly IReadRepository<TimeFrame> timeframeRep;
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<Chart> chartRep;
  private readonly IReadRepository<JoinedChart> joinedChartRep;
  private readonly IReadRepository<JoinedCandle> joinedCandleRep;
  private readonly IJoinedChartManager joinedChartManager;
  private readonly IMediator mediator;
  private readonly IJoinedChartFactory joinedChartFactory;
  private readonly ILogger<GetJoinedChartHandler> logger;

  public GetJoinedChartHandler(
   IReadRepository<TimeFrame> timeframeRep,
   IReadRepository<ent.Instrument> instrumentRep,
   IReadRepository<Chart> chartRep,
   IReadRepository<JoinedChart> joinedChartRep,
   IReadRepository<JoinedCandle> joinedCandleRep,
   IJoinedChartManager joinedChartManager,
   IMediator mediator,
   IJoinedChartFactory joinedChartFactory,
   ILogger<GetJoinedChartHandler> logger)
  {
    this.timeframeRep = timeframeRep;
    this.instrumentRep = instrumentRep;
    this.chartRep = chartRep;
    this.joinedChartRep = joinedChartRep;
    this.joinedCandleRep = joinedCandleRep;
    this.joinedChartManager = joinedChartManager;
    this.mediator = mediator;
    this.joinedChartFactory = joinedChartFactory;
    this.logger = logger;
  }
  public async Task<Result<GetJoinedCandleResponseDto>> Handle(GetJoinedChartRequestDto request, CancellationToken cancellationToken)
  {
    logger.LogDebug("Load exist joined chart");
    var joinedChart = await joinedChartRep.TryGetByAsync(request.instrumentId, request.stepTimeFrameId, request.targetTimeFrameId, cancellationToken);

    GetJoinedCandleResponseDto.EnumStatus responseStatus;
    if (joinedChart == null || request.from < joinedChart.FromDate || request.untill > joinedChart.UntillDate)
    {
      var baseChart = await chartRep.TryGetByAsync(request.instrumentId, request.stepTimeFrameId, cancellationToken);
      if (baseChart == null)
        return Result.NotFound(nameof(Chart));

      if (request.from < baseChart.FromDate || request.untill > baseChart.UntillDate)
        return Result.NotFound(nameof(Candle));

      if (joinedChart == null)
      {
        responseStatus = GetJoinedCandleResponseDto.EnumStatus.InProgress;
        joinedChart = await joinedChartFactory.CreateNewFor(baseChart.Id, request.targetTimeFrameId, cancellationToken);
      }
      else
      {
        responseStatus = GetJoinedCandleResponseDto.EnumStatus.PartlyReady;
      }

      await mediator.Publish(new EventRequestJoinedChartUpdate(joinedChart.Id), cancellationToken);
    }
    else
      responseStatus = GetJoinedCandleResponseDto.EnumStatus.Ready;

    IEnumerable<JoinedCandleDto> candlesArr = await joinedCandleRep.GetCandlesAsDtoAsync(request.from, request.untill, request.hideIntermediateCandles, joinedChart!);

    return Result.Success(
      new GetJoinedCandleResponseDto()
      {
        Status = responseStatus,
        JoinedCandles = candlesArr
      }
    );
  }

  public async Task<Result<JoinedChart?>> GetExistJoinedChartAsync(int instrumentId, int stepTimeFrameId, int targetTimeFrameId, CancellationToken cancellationToken)
  {
    var joinedChart = await joinedChartRep.GetByAsync(instrumentId, stepTimeFrameId, targetTimeFrameId, cancellationToken);
    if (joinedChart == null)
    {
      var notFound = new List<string>();
      var chartIsFound = await chartRep.ContainAsync(e => e.TimeFrameId == stepTimeFrameId && e.InstrumentId == instrumentId, cancellationToken);
      if (!chartIsFound)
      {
        if (!await instrumentRep.ContainIdAsync(instrumentId, cancellationToken))
          notFound.Add(nameof(ent.Instrument));
        if (!await timeframeRep.ContainIdAsync(stepTimeFrameId, cancellationToken))
          notFound.Add(nameof(TimeFrame));
        if (notFound.Count() == 0)
          notFound.Add(nameof(Chart));
      }

      if (!await timeframeRep.ContainIdAsync(targetTimeFrameId, cancellationToken))
        notFound.Add(nameof(TimeFrame));

      if (notFound.Count() != 0)
        return Result.NotFound(notFound.Distinct().ToArray());
    }

    return Result.Success(joinedChart);
  }
}

