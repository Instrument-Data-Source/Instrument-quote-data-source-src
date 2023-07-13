
using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;
using Ardalis.Result;
using FluentValidation.Results;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;

/// <summary>
/// Quotes of <see cref="ent.Instrument"/> 
/// <see cref="ADR-001 Decimal in candle value">ADR-001 Decimal in candle value</see>
/// </summary>
public partial class Candle
{
  protected Candle(DateTime dateTime,
              int openStore,
              int highStore,
              int lowStore,
              int closeStore,
              int volumeStore,
              int instrumentId,
              int timeFrameId)
  {

    DateTime = dateTime;
    OpenStore = openStore;
    HighStore = highStore;
    LowStore = lowStore;
    CloseStore = closeStore;
    VolumeStore = volumeStore;
    InstrumentId = instrumentId;
    TimeFrameId = timeFrameId;
  }
  private Candle(DateTime dateTime,
                int open,
                int high,
                int low,
                int close,
                int volume,
                [NotNull] ent.Instrument instrument,
                [NotNull] TimeFrame timeFrame,
                bool validate) : this(dateTime, open, high, low, close, volume, instrument.Id, timeFrame.Id)
  {
    Instrument = instrument;
    TimeFrame = timeFrame;
    if (validate)
      Validate();
  }
  public Candle(DateTime dateTime,
                int open,
                int high,
                int low,
                int close,
                int volume,
                [NotNull] ent.Instrument instrument,
                [NotNull] TimeFrame timeFrame) : this(dateTime, open, high, low, close, volume, instrument, timeFrame, true)
  {
  }

  public static Result<Candle> TryBuild(DateTime dateTime,
                                        int open,
                                        int high,
                                        int low,
                                        int close,
                                        int volume,
                                        [NotNull] ent.Instrument instrument,
                                        [NotNull] TimeFrame timeFrame)
  {
    var candle = new Candle(dateTime, open, high, low, close, volume, instrument, timeFrame, false);
    if (!candle.IsValid(out var result))
    {
      return result.ToResult();
    }

    return Result.Success(candle);
  }


  public static Result<Candle> TryBuild(DateTime dateTime,
                                        decimal open,
                                        decimal high,
                                        decimal low,
                                        decimal close,
                                        decimal volume,
                                        [NotNull] ent.Instrument instrument,
                                        [NotNull] TimeFrame timeFrame)
  {
    return TryBuild(
      dateTime,
      ToStoreValue(open, instrument.PriceDecimalLen, nameof(open)),
      ToStoreValue(high, instrument.PriceDecimalLen, nameof(high)),
      ToStoreValue(low, instrument.PriceDecimalLen, nameof(low)),
      ToStoreValue(close, instrument.PriceDecimalLen, nameof(close)),
      ToStoreValue(volume, instrument.PriceDecimalLen, nameof(volume)),
      instrument,
      timeFrame
    );
  }

  public Candle(DateTime dateTime,
                decimal open,
                decimal high,
                decimal low,
                decimal close,
                decimal volume,
                [NotNull] ent.Instrument instrument,
                [NotNull] TimeFrame timeFrame) : this
                (
                  dateTime,
                  ToStoreValue(open, instrument.PriceDecimalLen, nameof(open)),
                  ToStoreValue(high, instrument.PriceDecimalLen, nameof(high)),
                  ToStoreValue(low, instrument.PriceDecimalLen, nameof(low)),
                  ToStoreValue(close, instrument.PriceDecimalLen, nameof(close)),
                  ToStoreValue(volume, instrument.PriceDecimalLen, nameof(volume)),
                  instrument,
                  timeFrame
                )
  { }

  public static int ToStoreValue(decimal value, int decimalLen, string name = "")
  {
    var storeValue = value * (int)Math.Pow(10, decimalLen);
    if (storeValue % 1 != 0)
      throw new ArgumentOutOfRangeException(name, $"Decimal part in {name} longer than expected {decimalLen}.");
    return (int)storeValue;
  }
  /*
    private Candle(DateTime dateTime,
                  int open,
                  int high,
                  int low,
                  int close,
                  int volume,
                  [NotNull] ent.Instrument instrument,
                  [NotNull] TimeFrame timeFrame, bool validate) : this(dateTime, open, high, low, close, volume, instrument.Id, timeFrame.Id)
    {
      Instrument = instrument;
      TimeFrame = timeFrame;
      if (validate)
        Validate();
    }
  */
  /*
    public static bool TryBuild(DateTime dateTime,
                  int open,
                  int high,
                  int low,
                  int close,
                  int volume,
                  [NotNull] ent.Instrument instrument,
                  [NotNull] TimeFrame timeFrame, out Candle? candle, out ValidationResult validationResult)
    {
      var newCandle = new Candle(dateTime, open, high, low, close, volume, instrument, timeFrame, false);
      if (newCandle.IsValid(out validationResult))
      {
        candle = newCandle;
        return true;
      }
      else
      {
        candle = null;
        return false;
      }
    }
    */
}