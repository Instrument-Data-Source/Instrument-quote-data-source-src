namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;

public partial class Candle
{
  public interface IPayload
  {
    public DateTime DateTime { get; }
    public decimal Open { get; }
    public decimal High { get; }
    public decimal Low { get; }
    public decimal Close { get; }
    public decimal Volume { get; }
  }
}