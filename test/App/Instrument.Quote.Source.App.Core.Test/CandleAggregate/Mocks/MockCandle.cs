using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;

public class MockCandleFactory
{
  private readonly ent.Instrument instrument;
  private readonly TimeFrame timeFrame;

  public MockCandleFactory() : this(MockInstrument.Create(), TimeFrame.Enum.D1.ToEntity())
  {

  }
  public MockCandleFactory(ent.Instrument instrument, TimeFrame timeFrame)
  {
    this.instrument = instrument;
    this.timeFrame = timeFrame;
  }
  public Candle CreateRandomCandle(DateTime? dt = null)
  {
    var r = new Random();
    var open = r.Next(200, 500);
    return new Candle((dt ?? DateTime.Now).ToUniversalTime(),
      open,
      open + 100,
      open - 100,
      open + (r.Next(0, 100) - 50),
      r.Next(0, 100),
      this.instrument,
      this.timeFrame);
  }
  public IEnumerable<Candle> CreateRandomCandles(int count, DateTime? startDt = null)
  {
    List<Candle> _ret_arr = new();
    var dt = (startDt ?? new DateTime(2020, 1, 1)).ToUniversalTime();
    for (int i = 0; i < count; i++)
    {
      _ret_arr.Add(CreateRandomCandle(dt.AddDays(i)));
    }
    return _ret_arr;
  }

  public IEnumerable<Candle> CreateRandomCandles(DateTime fromDt, DateTime untillDT)
  {
    List<Candle> _ret_arr = new();
    var dt = fromDt.ToUniversalTime();
    while (dt < untillDT.ToUniversalTime())
    {
      _ret_arr.Add(CreateRandomCandle(dt));
      dt = dt.AddDays(1);
    }
    return _ret_arr;
  }
}