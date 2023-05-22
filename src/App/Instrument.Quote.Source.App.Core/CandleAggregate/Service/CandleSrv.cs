using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Repository;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Service;

public class CandleSrv : ICandleSrv
{
  private readonly IRepository<Candle> candleRep;
  private readonly IRepository<LoadedPeriod> loadedPeriodRep;
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly ILogger<CandleSrv> logger;

  public CandleSrv(IRepository<Candle> repository,
                    IRepository<LoadedPeriod> loadedPeriodRep,
                    IReadRepository<ent.Instrument> instrumentRep,
                    ILogger<CandleSrv>? logger = null)
  {
    this.candleRep = repository;
    this.loadedPeriodRep = loadedPeriodRep;
    this.instrumentRep = instrumentRep;
    this.logger = logger ?? NullLogger<CandleSrv>.Instance;
  }

  public async Task<int> AddAsync(int instrumentId, int timeFrameId, DateTime from, DateTime untill, IEnumerable<CandleDto> candles, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Prepare data to adding");
    var newPeriodEnt = await LoadedPeriod.BuildNewPeriodAsync(instrumentRep, instrumentId, timeFrameId, from, untill, candles, cancellationToken);
    var loadedPerEnt = await loadedPeriodRep.TryGetForAsync(instrumentId, timeFrameId, cancellationToken);
    if (loadedPerEnt == null)
    {

      logger.LogInformation("Add new period");
      await loadedPeriodRep.AddAsync(newPeriodEnt, cancellationToken: cancellationToken);
    }
    else
    {
      loadedPerEnt.Extend(newPeriodEnt);
      await loadedPeriodRep.SaveChangesAsync(cancellationToken);
    }
    return candles.Count();
  }

  public async Task<IEnumerable<CandleDto>> GetAsync(int instrumentId, int timeFrameId, DateTime? from = null, DateTime? untill = null)
  {
    var loadedPer = await loadedPeriodRep.GetForAsync(instrumentId, timeFrameId);
    if (from < loadedPer.FromDate || untill > loadedPer.UntillDate)
      throw new ArgumentOutOfRangeException("Period has not been loaded");

    IEnumerable<CandleDto> arr = await candleRep.Table
                                                .Where(e =>
                                                  e.InstrumentId == instrumentId &&
                                                  e.TimeFrameId == timeFrameId &&
                                                  e.DateTime >= from &&
                                                  e.DateTime < untill)
                                                .Include(e => e.Instrument)
                                                .Select(e => e.ToDto())
                                                .ToArrayAsync();
    return arr;
  }

  public async Task<IEnumerable<CandleDto>> GetAllAsync()
  {
    IEnumerable<CandleDto> arr = await candleRep.Table.Include(e => e.Instrument).Select(e => e.ToDto()).ToArrayAsync();
    return arr;
  }

  public async Task<PeriodResponseDto?> TryGetExistPeriodAsync(int instrumentId, int timeFrameId, CancellationToken cancellationToken = default)
  {
    var loadedPer = await loadedPeriodRep.TryGetForAsync(instrumentId, timeFrameId, cancellationToken);
    return loadedPer != null ? loadedPer.ToDto() : null;
  }

  public async Task<IReadOnlyDictionary<string, PeriodResponseDto>> GetExistPeriodAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    return await loadedPeriodRep.Table
                  .Where(e => e.InstrumentId == instrumentId)
                  .ToDictionaryAsync(e => Enum.GetName((TimeFrame.Enum)e.TimeFrameId)!, e => e.ToDto(), cancellationToken);
  }
}