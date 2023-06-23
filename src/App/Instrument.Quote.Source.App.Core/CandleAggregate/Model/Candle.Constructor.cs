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
  public Candle(DateTime dateTime,
                int open,
                int high,
                int low,
                int close,
                int volume,
                [NotNull] ent.Instrument instrument,
                [NotNull] TimeFrame timeFrame)
  {
    Guard.Against.Null(instrument);
    Guard.Against.Null(timeFrame);

    DateTime = dateTime;
    OpenStore = open;
    HighStore = high;
    LowStore = low;
    CloseStore = close;
    VolumeStore = volume;
    Instrument = instrument;
    TimeFrame = timeFrame;
    Validate();
  }
}