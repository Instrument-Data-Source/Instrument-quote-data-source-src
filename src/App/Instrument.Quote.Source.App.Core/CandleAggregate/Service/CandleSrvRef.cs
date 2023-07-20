using Ardalis.Result;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Service;
public class CandleSrvRef
{
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<TimeFrame> timeframeRep;
  private readonly IRepository<LoadedPeriod> loadedPeriodRep;
  private readonly IRepository<Candle> candleRep;
  private readonly ILogger<CandleSrvRef> logger;

  public CandleSrvRef(IReadRepository<ent.Instrument> instrumentRep,
                    IReadRepository<TimeFrame> timeframeRep,
                    IRepository<LoadedPeriod> loadedPeriodRep,
                    IRepository<Candle> candleRep,
                    ILogger<CandleSrvRef>? logger = null)
  {
    this.instrumentRep = instrumentRep;
    this.timeframeRep = timeframeRep;
    this.loadedPeriodRep = loadedPeriodRep;
    this.candleRep = candleRep;
    this.logger = logger ?? NullLogger<CandleSrvRef>.Instance;
  }

  public async Task<Result<int>> AddAsync(NewPeriodDto addCandlesDto, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Load related entities from DB");
    var instrumentEnt = await instrumentRep.GetByIdAsync(addCandlesDto.InstrumentId, cancellationToken);
    var timeFrameEnt = await timeframeRep.GetByIdAsync(addCandlesDto.TimeFrameId, cancellationToken);

    logger.LogDebug("Convert DTO into new Entity");

    var candles = addCandlesDto.Candles.Select(c => new Candle(c.DateTime, c.Open, c.High, c.Low, c.Close, c.Volume, instrumentEnt, timeFrameEnt));
    var newLoadedPer = new LoadedPeriod(addCandlesDto.FromDate,
                                        addCandlesDto.UntillDate,
                                        instrumentEnt,
                                        timeFrameEnt, candles);

    logger.LogDebug("Searching exist period");
    var existLoadedPer = await loadedPeriodRep.TryGetForAsync(instrumentEnt.Id, timeFrameEnt.Id, cancellationToken);
    if (existLoadedPer == null)
    {
      logger.LogInformation("Add first period");
      await loadedPeriodRep.AddAsync(newLoadedPer, cancellationToken: cancellationToken);
      return Result.Success(newLoadedPer.Candles.Count());
    }
    else
    {
      logger.LogInformation("Extend exist period");
      var addedPeriodRes = existLoadedPer.TryExtend(newLoadedPer);
      if (!addedPeriodRes.IsSuccess)
        return addedPeriodRes.Repack<int>();

      await loadedPeriodRep.SaveChangesAsync(cancellationToken);
      return Result.Success(addedPeriodRes.Value.Candles.Count());
    }
  }

  public async Task<Result<IEnumerable<CandleDto>>> GetAsync(int instrumentId, int timeFrameId, DateTime? from = null, DateTime? untill = null, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Load exist period");
    var existPeriodRes = await GetExistPeriodAsync(instrumentId, timeFrameId, cancellationToken);
    if (!existPeriodRes.IsSuccess)
      return existPeriodRes.Repack<IEnumerable<CandleDto>>();

    var loadedPer = existPeriodRes.Value;

    logger.LogDebug("Check loaded period");
    if (from < loadedPer.FromDate || untill > loadedPer.UntillDate)
      return Result.Error("Period has not been loaded");

    IEnumerable<CandleDto> arr = await candleRep.Table
                                                .Where(e =>
                                                  e.InstrumentId == instrumentId &&
                                                  e.TimeFrameId == timeFrameId &&
                                                  e.DateTime >= from &&
                                                  e.DateTime < untill)
                                                .Include(e => e.Instrument)
                                                .Select(e => e.ToDto())
                                                .ToArrayAsync();
    return Result.Success(arr);
  }

  public async Task<Result<IReadOnlyDictionary<string, PeriodResponseDto>>> GetExistPeriodsAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    var periods = await loadedPeriodRep.Table.Where(e => e.InstrumentId == instrumentId).ToArrayAsync(cancellationToken);
    if (periods.Count() == 0)
      if (!await instrumentRep.ContainIdAsync(instrumentId, cancellationToken))
        return Result.NotFound(nameof(Instrument));
      else
        return Result.NotFound(nameof(LoadedPeriod));

    IReadOnlyDictionary<string, PeriodResponseDto> _ret_dto = periods.ToDictionary(e => Enum.GetName((TimeFrame.Enum)e.TimeFrameId)!, e => e.ToDto());
    return Result.Success(_ret_dto);
  }

  public async Task<Result<PeriodResponseDto>> GetExistPeriodAsync(int instrumentId, int timeFrameId, CancellationToken cancellationToken = default)
  {
    var loadedPer = await loadedPeriodRep.TryGetForAsync(instrumentId, timeFrameId, cancellationToken);
    if (loadedPer == null)
    {
      var isInstExist = await instrumentRep.ContainIdAsync(instrumentId, cancellationToken);
      var isTfExist = await timeframeRep.ContainIdAsync(timeFrameId, cancellationToken);
      var notFound = new List<string>();
      if (!isInstExist)
        notFound.Add(nameof(Instrument));
      if (!isTfExist)
        notFound.Add(nameof(TimeFrame));
      if (notFound.Count > 0)
        return Result.NotFound(notFound.ToArray());
      return Result.NotFound(nameof(LoadedPeriod));
    }

    return Result.Success(loadedPer.ToDto());
  }
}