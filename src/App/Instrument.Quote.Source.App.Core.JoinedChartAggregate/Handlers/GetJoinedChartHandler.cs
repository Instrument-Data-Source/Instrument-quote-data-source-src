using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.ChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Instrument.Quote.Source.Shared.Result.Extension;
using MediatR;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Microsoft.EntityFrameworkCore;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Events;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Handlers;

public class GetJoinedChartHandler : IRequestHandler<GetJoinedChartRequestDto, Result<GetJoinedCandleResponseDto>>
{
  private readonly IReadRepository<TimeFrame> timeframeRep;
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<Chart> chartRep;
  private readonly IReadRepository<JoinedChart> joinedChartRep;
  private readonly IReadRepository<JoinedCandle> joinedCandleRep;
  private readonly IJoindeCandleMapper joindeCandleMapper;
  private readonly IJoinedChartManager joinedChartManager;
  private readonly IMediator mediator;
  private readonly JoinedChart.Factory joinedChartFactory;
  private readonly ILogger<GetJoinedChartHandler> logger;

  public GetJoinedChartHandler(
   IReadRepository<TimeFrame> timeframeRep,
   IReadRepository<ent.Instrument> instrumentRep,
   IReadRepository<Chart> chartRep,
   IReadRepository<JoinedChart> joinedChartRep,
   IReadRepository<JoinedCandle> joinedCandleRep,
   IJoindeCandleMapper joindeCandleMapper,
   IJoinedChartManager joinedChartManager,
   IMediator mediator,
   JoinedChart.Factory joinedChartFactory,
   ILogger<GetJoinedChartHandler> logger)
  {
    this.timeframeRep = timeframeRep;
    this.instrumentRep = instrumentRep;
    this.chartRep = chartRep;
    this.joinedChartRep = joinedChartRep;
    this.joinedCandleRep = joinedCandleRep;
    this.joindeCandleMapper = joindeCandleMapper;
    this.joinedChartManager = joinedChartManager;
    this.mediator = mediator;
    this.joinedChartFactory = joinedChartFactory;
    this.logger = logger;
  }
  public async Task<Result<GetJoinedCandleResponseDto>> Handle(GetJoinedChartRequestDto request, CancellationToken cancellationToken)
  {
    logger.LogDebug("Load exist joined chart");
    var joinedChartResult = await GetExistJoinedChartAsync(request.instrumentId, request.stepTimeFrameId, request.targetTimeFrameId, cancellationToken);
    if (!joinedChartResult.IsSuccess)
      return joinedChartResult.Repack<GetJoinedCandleResponseDto>();

    var joinedChart = joinedChartResult.Value;
    GetJoinedCandleResponseDto.EnumStatus responseStatus;
    if (joinedChart == null || request.from < joinedChart.FromDate || request.untill > joinedChart.UntillDate)
    {

      var baseChart = await chartRep.GetByAsync(request.instrumentId, request.stepTimeFrameId, cancellationToken);
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

      await mediator.Publish(new JoinedChartUpdateRequested(joinedChart.Id), cancellationToken);
    }
    else
      responseStatus = GetJoinedCandleResponseDto.EnumStatus.Ready;

    IEnumerable<JoinedCandleDto> candlesArr = await SelectJoinedCandles(request.from, request.untill, request.hideIntermediateCandles, joinedChart!);

    return Result.Success(
      new GetJoinedCandleResponseDto()
      {
        Status = responseStatus,
        JoinedCandles = candlesArr
      }
    );
  }

  private async Task<IEnumerable<JoinedCandleDto>> SelectJoinedCandles(DateTime from, DateTime untill, bool hideIntermediateCandles, JoinedChart joinedChart)
  {
    var query = joinedCandleRep.Table.Where(e => e.JoinedChartId == joinedChart.Id &&
                                                     e.StepDateTime >= from &&
                                                     e.StepDateTime < untill);
    if (hideIntermediateCandles)
      query = query.Where(e => e.IsLast);
    return (await query.ToArrayAsync()).Select(this.joindeCandleMapper.map);
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

