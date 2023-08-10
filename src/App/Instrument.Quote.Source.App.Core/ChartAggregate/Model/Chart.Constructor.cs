using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.DateTimePeriod;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;

public partial class Chart
{
  protected Chart(DateTime fromDate,
                          DateTime untillDate,
                          int instrumentId,
                          int timeFrameId)
  {
    FromDate = fromDate;
    UntillDate = untillDate;
    InstrumentId = instrumentId;
    TimeFrameId = timeFrameId;
  }
  public Chart(DateTime from,
                DateTime untill,
                ent.Instrument instrument,
                TimeFrame timeFrame) : this(from, untill, instrument.Id, timeFrame.Id)
  {
    Instrument = instrument;
    TimeFrame = timeFrame;
    Validate();
  }

  public Chart(DateTimePeriod period,
              ent.Instrument instrument,
              TimeFrame timeFrame) : this(period.From, period.Untill, instrument, timeFrame) { }

}