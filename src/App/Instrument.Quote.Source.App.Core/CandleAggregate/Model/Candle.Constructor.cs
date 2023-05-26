using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;

/// <summary>
/// Quotes of <see cref="ent.Instrument"/> 
/// <see cref="ADR-001 Decimal in candle value">ADR-001 Decimal in candle value</see>
/// </summary>
public partial class Candle : EntityBase
{
  public Candle(DateTime dateTime,
                int openStore,
                int highStore,
                int lowStore,
                int closeStore,
                int volumeStore,
                int timeFrameId,
                ent.Instrument instrument)
        : this(dateTime,
              openStore,
              highStore,
              lowStore,
              closeStore,
              volumeStore,
              timeFrameId,
              instrument.Id)
  {
    Instrument = instrument;
  }
  public Candle(DateTime dateTime,
                int openStore,
                int highStore,
                int lowStore,
                int closeStore,
                int volumeStore,
                TimeFrame timeFrame,
                int instrumentId)
        : this(dateTime,
              openStore,
              highStore,
              lowStore,
              closeStore,
              volumeStore,
              timeFrame.Id,
              instrumentId)
  {
    TimeFrame = timeFrame;
  }
  public Candle(DateTime dateTime,
                int openStore,
                int highStore,
                int lowStore,
                int closeStore,
                int volumeStore,
                TimeFrame timeFrame,
                ent.Instrument instrument)
        : this(dateTime,
              openStore,
              highStore,
              lowStore,
              closeStore,
              volumeStore,
              timeFrame.Id,
              instrument.Id)
  {
    TimeFrame = timeFrame;
    Instrument = instrument;
  }

  public Candle(DateTime dateTime,
                int openStore,
                int highStore,
                int lowStore,
                int closeStore,
                int volumeStore,
                TimeFrame.Enum timeFrameEnumId,
                int instrumentId)
            : this(dateTime, openStore, highStore, lowStore, closeStore, volumeStore, (int)timeFrameEnumId, instrumentId)
  { }

}