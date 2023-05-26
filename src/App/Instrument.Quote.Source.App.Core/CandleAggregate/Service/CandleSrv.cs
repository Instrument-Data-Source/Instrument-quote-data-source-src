using Ardalis.Result;
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

  public async Task<Result<int>> AddAsync(int instrumentId, int timeFrameId, DateTime from, DateTime untill, IEnumerable<CandleDto> candles, CancellationToken cancellationToken = default)
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
    return Result.Success(candles.Count());
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

  public async Task<Result<PeriodResponseDto>> TryGetExistPeriodAsync(int instrumentId, int timeFrameId, CancellationToken cancellationToken = default)
  {
    var loadedPer = await loadedPeriodRep.TryGetForAsync(instrumentId, timeFrameId, cancellationToken);
    if (loadedPer == null)
      return Result.NotFound();

    return Result.Success(loadedPer.ToDto());
  }

  public async Task<Result<IReadOnlyDictionary<string, PeriodResponseDto>>> GetExistPeriodAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    var periods = await loadedPeriodRep.Table.Where(e => e.InstrumentId == instrumentId).ToArrayAsync(cancellationToken);
    if (periods.Count() == 0 && !await instrumentRep.ContainIdAsync(instrumentId, cancellationToken))
      return Result.NotFound();

    IReadOnlyDictionary<string, PeriodResponseDto> _ret_dto = periods.ToDictionary(e => Enum.GetName((TimeFrame.Enum)e.TimeFrameId)!, e => e.ToDto());
    return Result.Success(_ret_dto);
  }
  /*
    public async Task<Result<PeriodResponseDto>> TryGetExistPeriodAsync(string instrumentStr, string timeframeStr, CancellationToken cancellationToken = default)
    {
      var findedEnt = await instrumentRep.Table.Include(e => e.InstrumentType).SingleOrDefaultAsync(e => e.Code == instrumentStr, cancellationToken);
      int instrumentId = -1;
      if (findedEnt != null)
        instrumentId = findedEnt.Id;
      else if (!Int32.TryParse(instrumentStr, out instrumentId))
        return Result.NotFound();

      int timeframeId = -1;
      if (!Int32.TryParse(timeframeStr, out timeframeId))
      {
        TimeFrame.Enum? timeframeParseId = Enum.GetValues<TimeFrame.Enum>().SingleOrDefault(e => e.ToString() == timeframeStr);
        if (timeframeParseId != null)
        {
          timeframeId = (int)timeframeParseId;
        }
        else
          return Result.NotFound();
      }

      return await TryGetExistPeriodAsync(instrumentId, timeframeId, cancellationToken);
    }*/
}