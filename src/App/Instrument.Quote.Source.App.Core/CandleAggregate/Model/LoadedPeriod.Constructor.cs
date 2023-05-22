using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;
public partial class LoadedPeriod : EntityBase
{
  public LoadedPeriod(int instrumentId,
                      TimeFrame.Enum timeFrameEnumId,
                      DateTime fromDate,
                      DateTime untillDate)
        : this(instrumentId, (int)timeFrameEnumId, fromDate, untillDate)
  { }
  /// <summary>
  /// Create New LoadedPeriod base on import DTO
  /// </summary>
  /// <param name="instrumentRep">Repository of Instruments</param>
  /// <param name="instrumentId">Instrument Id</param>
  /// <param name="timeFrameId">TimeFrameId</param>
  /// <param name="from">DateTime From</param>
  /// <param name="untill">DateTime unTill</param>
  /// <param name="candles">loaded condles</param>
  /// <returns></returns>
  public static async Task<LoadedPeriod> BuildNewPeriodAsync(IReadRepository<ent.Instrument> instrumentRep, int instrumentId, int timeFrameId, DateTime from, DateTime untill, IEnumerable<CandleDto> candles, CancellationToken cancellationToken = default)
  {
    var instrument = await instrumentRep.GetByIdAsync(instrumentId, cancellationToken);
    var candleEntities = candles.Select(e => e.ToEntity(instrument, timeFrameId)).ToArray();
    return BuildNewPeriod(instrumentId, timeFrameId, from, untill, candleEntities);
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
  public static LoadedPeriod BuildNewPeriod(int instrumentId, int timeFrameId, DateTime from, DateTime untill, IEnumerable<Candle> candles)
  {
    return new LoadedPeriod(instrumentId, timeFrameId, from, untill).AddCandles(candles);
  }
  public static LoadedPeriod BuildNewPeriod(int instrumentId, TimeFrame.Enum timeFrameEnumId, DateTime from, DateTime untill, IEnumerable<Candle> candles)
  {
    return new LoadedPeriod(instrumentId, timeFrameEnumId, from, untill).AddCandles(candles);
  }
}