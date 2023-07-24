using Ardalis.Result;
using FluentValidation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Service;

public class CandlesSrv : ICandleSrv
{
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<TimeFrame> timeframeRep;
  private readonly IReadRepository<Candle> candleRep;
  private readonly IRepository<Chart> chartRep;
  private readonly ILogger<CandlesSrv> logger;

  public CandlesSrv(
      IRepository<Chart> chartRep,
      IReadRepository<ent.Instrument> instrumentRep,
      IReadRepository<TimeFrame> timeframeRep,
      IReadRepository<Candle> candleRep,
      ILogger<CandlesSrv> logger)
  {
    this.instrumentRep = instrumentRep;
    this.timeframeRep = timeframeRep;
    this.candleRep = candleRep;
    this.chartRep = chartRep;
    this.logger = logger;
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
    if (existChart == null)
    {
      logger.LogInformation("Add first period");
      await chartRep.AddAsync(newChart, cancellationToken: cancellationToken);
      return Result.Success(newChart.Candles.Count());
    }
    else
    {
      logger.LogInformation("Extend exist period");
      var extendRes = existChart.Extend(newChart);
      if (!extendRes.IsSuccess)
        return extendRes;

      await chartRep.SaveChangesAsync(cancellationToken);
      return extendRes.Value;
    }
  }

  public async Task<Result<IEnumerable<CandleDto>>> GetAsync(int instrumentId, int timeFrameId, DateTime from, DateTime untill, CancellationToken cancellationToken = default)
  {
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

  public async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(int instrumentId, int baseTimeFrameId, int chartTimeFrameId, DateTime from, DateTime untill, bool addIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Load exist chart");
    var chartResult = await GetExistChartAsync(instrumentId, baseTimeFrameId, cancellationToken);
    if (!chartResult.IsSuccess)
      return chartResult.Repack<IEnumerable<JoinedCandleDto>>();
    var chart = chartResult.Value;

    if (from < chart.FromDate || untill > chart.UntillDate)
      return Result.NotFound(nameof(Candle));

    var targetTf = await timeframeRep.TryGetByIdAsync(chartTimeFrameId, cancellationToken);
    if (targetTf == null)
      return Result.NotFound(nameof(TimeFrame));

    throw new NotImplementedException();
  }
}