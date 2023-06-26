using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Ardalis.GuardClauses;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

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

  public Candle(DateTime dateTime,
                int open,
                int high,
                int low,
                int close,
                int volume,
                [NotNull] ent.Instrument instrument,
                [NotNull] TimeFrame timeFrame) : this(dateTime, open, high, low, close, volume, instrument.Id, timeFrame.Id)
  {
    Instrument = instrument;
    TimeFrame = timeFrame;
    Validate();
  }
}