using Ardalis.GuardClauses;
using Ardalis.Result;
using FluentValidation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Events;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Core.Validation;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Instrument.Quote.Source.Shared.Result.Extension;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Service;

public class CandlesSrv : ICandleSrv
{
  private readonly IRepository<Chart> chartRep;
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<TimeFrame> timeframeRep;
  private readonly IRepository<Candle> candleRep;
  private readonly IMediator mediator;
  private readonly ILogger<CandlesSrv> logger;

  public CandlesSrv(
      IRepository<Chart> chartRep,
      IReadRepository<ent.Instrument> instrumentRep,
      IReadRepository<TimeFrame> timeframeRep,
      IRepository<Candle> candleRep,
      IMediator mediator,
      ILogger<CandlesSrv> logger)
  {
    this.chartRep = chartRep;
    this.instrumentRep = instrumentRep;
    this.timeframeRep = timeframeRep;
    this.candleRep = candleRep;
    this.mediator = mediator;
    this.logger = logger;
  }

  public async Task<Result<int>> AddAsync(int instrumentId, int timeFrameId, UploadedCandlesDto uploadedCandlesDto, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Get related entities");
    var getEntityResult = await getEntityAsync(instrumentId, timeFrameId, cancellationToken);
    if (!getEntityResult.IsSuccess)
      return getEntityResult.Repack<int>();
    var instrument = await instrumentRep.GetByIdAsync(instrumentId, cancellationToken);
    var timeframe = await timeframeRep.GetByIdAsync(timeFrameId, cancellationToken);

    logger.LogDebug("Convert dto");
    var mapResult = uploadedCandlesDto.ToEntity(instrument, timeframe);
    if (!mapResult.IsSuccess)
      return mapResult.Repack<int>();
    var newChart = mapResult.Value;

    logger.LogDebug("Searching exist period");
    var existChart = await chartRep.TryGetForAsync(instrumentId, timeFrameId, cancellationToken);

    Result<int> result;
    if (existChart == null)
    {
      logger.LogInformation("Add first period");
      await chartRep.AddAsync(newChart, cancellationToken: cancellationToken);
      existChart = newChart;
      result = Result.Success(newChart.Candles.Count());
    }
    else
    {
      logger.LogInformation("Extend exist period");
      var extendRes = await new Chart.Manager(candleRep).Extend(existChart, newChart);
      if (!extendRes.IsSuccess)
        return extendRes;

      await chartRep.SaveChangesAsync(cancellationToken);
      result = extendRes;
    }

    await mediator.Publish(new CandlesAddedNotification(existChart, newChart.FromDate, newChart.UntillDate));
    return result;
  }

  public async Task<Result<IEnumerable<CandleDto>>> GetAsync(int instrumentId, int timeFrameId, [UTCKind] DateTime from, [UTCKind] DateTime untill, CancellationToken cancellationToken = default)
  {
    Guard.Against.AgainstExpression(UTCKindAttribute.IsUTC, from, $"{nameof(from)} must be in UTC kind");
    Guard.Against.AgainstExpression(UTCKindAttribute.IsUTC, untill, $"{nameof(untill)} must be in UTC kind");

    logger.LogDebug("Load exist chart");
    var chartResult = await GetExistChartAsync(instrumentId, timeFrameId, cancellationToken);
    if (!chartResult.IsSuccess)
      return chartResult.Repack<IEnumerable<CandleDto>>();
    var chart = chartResult.Value;

    if (from < chart.FromDate || untill > chart.UntillDate)
      return Result.NotFound(nameof(Candle));

    var candleMapper = new CandleMapper(chart);

    IEnumerable<Candle> arr = await candleRep.Table
                                                 .Where(e =>
                                                   e.ChartId == chart.Id &&
                                                   e.DateTime >= from &&
                                                   e.DateTime < untill)
                                                 .ToArrayAsync();
    return Result.Success(arr.Select(candleMapper.map));
  }

  public async Task<Result<(ent.Instrument, TimeFrame)>> getEntityAsync(int instrumentId, int timeFrameId, CancellationToken cancellationToken)
  {
    var instrument = await instrumentRep.TryGetByIdAsync(instrumentId, cancellationToken);
    var timeframe = await timeframeRep.TryGetByIdAsync(timeFrameId, cancellationToken);
    var notFound = new List<string>();

    if (instrument == null)
      notFound.Add(nameof(ent.Instrument));
    if (timeframe == null)
      notFound.Add(nameof(TimeFrame));

    if (notFound.Count != 0)
      return Result.NotFound(notFound.ToArray());

    return Result.Success((instrument!, timeframe!));
  }

  public async Task<Result<Chart>> GetExistChartAsync(int instrumentId, int timeFrameId, CancellationToken cancellationToken = default)
  {
    var chart = await chartRep.Table.Include(c => c.Instrument).SingleOrDefaultAsync(e => e.TimeFrameId == timeFrameId && e.InstrumentId == instrumentId, cancellationToken);
    if (chart == null)
    {
      var notFound = new List<string>();
      if (!await instrumentRep.ContainIdAsync(instrumentId, cancellationToken))
        notFound.Add(nameof(ent.Instrument));
      if (!await timeframeRep.ContainIdAsync(timeFrameId, cancellationToken))
        notFound.Add(nameof(TimeFrame));
      if (notFound.Count() == 0)
        notFound.Add(nameof(Chart));
      return Result.NotFound(notFound.ToArray());
    }

    return Result.Success(chart);
  }
}