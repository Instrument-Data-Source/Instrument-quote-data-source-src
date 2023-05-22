using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.TestData;

public class CandleFactory
{
  public static Candle RandomCandle(DateTime? dt = null)
  {
    var r = new Random();
    var instrument = new ent.Instrument("inst", "i", 2, 2, 1);
    var open = r.Next(200, 500);
    return new Candle(dt ?? DateTime.UtcNow,
    open,
    open + 200,
    open - 100,
    open + (r.Next(0, 100) - 50),
    r.Next(1, 100),
    (int)TimeFrame.Enum.D1,
    instrument);
  }
  public static IEnumerable<Candle> RandomCandles(int count, DateTime? startDt = null)
  {
    List<Candle> _ret_arr = new();
    var dt = startDt ?? new DateTime(2020, 1, 1).ToUniversalTime();
    for (int i = 0; i < count; i++)
    {
      _ret_arr.Add(RandomCandle(dt.AddDays(i)));
    }
    return _ret_arr;
  }
}