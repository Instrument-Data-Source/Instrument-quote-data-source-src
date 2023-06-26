using FluentValidation.Results;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;
public partial class LoadedPeriod : EntityBaseExt
{

  protected LoadedPeriod(DateTime fromDate,
                        DateTime untillDate,
                        int instrumentId,
                        int timeFrameId)
  {
    FromDate = fromDate;
    UntillDate = untillDate;
    InstrumentId = instrumentId;
    TimeFrameId = timeFrameId;
  }

  public LoadedPeriod(DateTime from,
                      DateTime untill,
                      ent.Instrument instrument,
                      TimeFrame timeFrame,
                      IEnumerable<Candle> candles) : this(from, untill, instrument.Id, timeFrame.Id)
  {
    Instrument = instrument;
    TimeFrame = timeFrame;
    _candles = candles.ToList();
    Validate();
  }

  public IQueryable<LoadedPeriod> BuildMock()
  {
    throw new NotImplementedException();
  }

  /*
  /// <summary>
  /// Create New LoadedPeriod base on import DTO
  /// </summary>
  /// <param name="instrumentRep">Repository of Instruments</param>
  /// <param name="instrumentId">Instrument Id</param>
  /// <param name="timeFrameRep">Repository of TimeFrames</param>
  /// <param name="timeFrameId">TimeFrameId</param>
  /// <param name="from">DateTime From</param>
  /// <param name="untill">DateTime unTill</param>
  /// <param name="candles">loaded condles</param>
  /// <returns></returns>
  public static async Task<LoadedPeriod> BuildNewPeriodAsync(
      IReadRepository<ent.Instrument> instrumentRep, int instrumentId,
      IReadRepository<TimeFrame> timeFrameRep, int timeFrameId,
      DateTime from, DateTime untill, IEnumerable<CandleDto> candles, CancellationToken cancellationToken = default)
  {
    var instrument = await instrumentRep.GetByIdAsync(instrumentId, cancellationToken);
    var timeframe = await timeFrameRep.GetByIdAsync(timeFrameId, cancellationToken);

    return BuildNewPeriod(instrument, timeframe, from, untill, candles);
  }

  /// <summary>
  /// Create New LoadedPeriod base on import DTO
  /// </summary>
  /// <param name="instrument">Instrument</param>
  /// <param name="timeFrame">TimeFrame</param>
  /// <param name="from">DateTime From</param>
  /// <param name="untill">DateTime unTill</param>
  /// <param name="candles">loaded condles</param>
  /// <returns></returns>
  public static LoadedPeriod BuildNewPeriod(
      ent.Instrument instrument,
      TimeFrame timeFrame,
      DateTime from, DateTime untill, IEnumerable<CandleDto> candles)
  {
    var candleEntities = candles.Select(e => e.ToEntity(instrument, timeFrame.Id)).ToArray();
    return BuildNewPeriod(instrument, timeFrame, from, untill, candleEntities);
  }

  /// <summary>
  /// Create New LoadedPeriod base on new Candles
  /// </summary>
  /// <param name="instrumentId">Instrument Id</param>
  /// <param name="timeFrameEnumId">TimeFrameId</param>
  /// <param name="from">DateTime From</param>
  /// <param name="untill">DateTime unTill</param>
  /// <param name="candles">loaded condles</param>
  /// <returns></returns>
  public static LoadedPeriod BuildNewPeriod(ent.Instrument instrument, TimeFrame timeFrame, DateTime from, DateTime untill, IEnumerable<Candle> candles)
  {
    return new LoadedPeriod(instrument, timeFrame, from, untill).AddCandles(candles);

  }
  /*
    public static ValidationResult Create(ent.Instrument instrument, TimeFrame timeFrame,
                                          DateTime from, DateTime untill, IEnumerable<Candle> candles,
                                          out LoadedPeriod entity)
    {
      var newPeriod = new LoadedPeriod(instrument, timeFrame, from, untill).AddCandles(candles);
      var validationResult = new LoadedPeriod.Validator().Validate(newPeriod);

    }
    */
}