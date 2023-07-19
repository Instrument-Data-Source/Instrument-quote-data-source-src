using System.ComponentModel.DataAnnotations;
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

public class CandleSrv : ICandleSrv
{
  private readonly IRepository<Candle> candleRep;
  private readonly IRepository<LoadedPeriod> loadedPeriodRep;
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<TimeFrame> timeframeRep;
  private readonly ILogger<CandleSrv> logger;

  public CandleSrv(IRepository<Candle> repository,
                    IRepository<LoadedPeriod> loadedPeriodRep,
                    IReadRepository<ent.Instrument> instrumentRep,
                    IReadRepository<TimeFrame> timeframeRep,
                    ILogger<CandleSrv>? logger = null)
  {
    this.candleRep = repository;
    this.loadedPeriodRep = loadedPeriodRep;
    this.instrumentRep = instrumentRep;
    this.timeframeRep = timeframeRep;
    this.logger = logger ?? NullLogger<CandleSrv>.Instance;
  }
  private async Task<Result<(ent.Instrument, TimeFrame)>> LoadRelatietEntity(int instrumentId, int timeFrameId, CancellationToken cancellationToken = default)
  {
    List<ValidationResult> validationResults = new List<ValidationResult>();
    logger.LogDebug("Load related entities from DB");
    var instrument = await instrumentRep.TryGetByIdAsync(instrumentId, cancellationToken);
    var timeFrame = await timeframeRep.TryGetByIdAsync(timeFrameId, cancellationToken);

    if (instrument == null)
    {
      validationResults.Add(new ValidationResult("Instrument by ID not found", new[] { nameof(instrumentId) }));
    }
    if (timeFrame == null)
    {
      validationResults.Add(new ValidationResult("TimeFrame by ID not found", new[] { nameof(timeFrameId) }));
    }

    if (instrument == null || timeFrame == null)
      return Result.NotFound(validationResults.Select(v => v.ErrorMessage).ToArray());

    return Result.Success((instrument, timeFrame));
  }

  private Result<LoadedPeriod> TryConvert(NewPeriodDto addCandlesDto, ent.Instrument instrument, TimeFrame timeFrame)
  {
    logger.LogDebug("Convert candles from DTO to new Candle Entity");
    if (!addCandlesDto.Candles.TryConvertToEntity(instrument, timeFrame, out var candles, out var validationResult))
    {
      return Result.Invalid(validationResult.Errors.ToResultErrorList());
    }

    logger.LogDebug("Convert dto into new LoadedPeriod entity");
    var result = LoadedPeriod.TryBuild(addCandlesDto.FromDate, addCandlesDto.UntillDate, instrument, timeFrame);
    result.Value.AddCandles(candles);
    return result;
  }
  public async Task<Result<int>> AddAsync(NewPeriodDto addCandlesDto, CancellationToken cancellationToken = default)
  {

    var loadingResult = await LoadRelatietEntity(addCandlesDto.InstrumentId, addCandlesDto.TimeFrameId, cancellationToken);
    if (!loadingResult.IsSuccess)
      return loadingResult.Repack<(ent.Instrument, TimeFrame), int>();

    (var instrument, var timeFrame) = loadingResult.Value;

    var convertResult = TryConvert(addCandlesDto, instrument, timeFrame);
    if (!convertResult.IsSuccess)
      return convertResult.Repack<LoadedPeriod, int>();

    var newLoadedPer = convertResult.Value;

    logger.LogDebug("Searching exist period");
    var existLoadedPer = await loadedPeriodRep.TryGetForAsync(instrument.Id, timeFrame.Id, cancellationToken);
    if (existLoadedPer == null)
    {
      logger.LogDebug("Add first period");
      await loadedPeriodRep.AddAsync(newLoadedPer, cancellationToken: cancellationToken);
    }
    else
    {
      existLoadedPer.Extend(newLoadedPer);
      await loadedPeriodRep.SaveChangesAsync(cancellationToken);
    }

    return Result.Success(newLoadedPer.Candles.Count());
  }


  /*
   private async Task<(LoadedPeriod? existPeriod, LoadedPeriod newPeriod)> convertDtoAsync(AddCandlesDto addCandlesDto, CancellationToken cancellationToken = default)
   {

     logger.LogInformation("Searching entities by Id");
     var instrument = await instrumentRep.TryGetByIdAsync(addCandlesDto.instrumentId, cancellationToken);
     var timeFrame = await timeframeRep.TryGetByIdAsync(addCandlesDto.timeFrameId, cancellationToken);

     var newPeriodEnt = LoadedPeriod.BuildNewPeriod(instrument, timeFrame, addCandlesDto.From, addCandlesDto.Untill, addCandlesDto.Candles);

     var loadedPerEnt = await loadedPeriodRep.TryGetForAsync(addCandlesDto.instrumentId, addCandlesDto.timeFrameId, cancellationToken);
     return (loadedPerEnt, newPeriodEnt);


   }
  */
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