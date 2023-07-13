namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;

public partial class LoadedPeriod
{
  public interface IPayload
  {

    public int InstrumentId { get; }
    public int TimeFrameId { get; }
    public DateTime FromDate { get; }
    public DateTime UntillDate { get; }
  }
}