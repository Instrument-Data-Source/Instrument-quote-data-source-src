using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

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

  public class Factory
  {
    private readonly IServiceProvider sp;

    public Factory(IServiceProvider sp)
    {
      this.sp = sp;
    }

    public Chart Build(DateTime from,
                      DateTime untill,
                      ent.Instrument instrument,
                      TimeFrame timeFrame)
    {
      var entity = new Chart(from, untill, instrument, timeFrame);
      entity.SetServiceProvider(sp);
      return entity;
    }
  }
}