using System.Diagnostics.CodeAnalysis;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;

public class MockCandleFactory : absMockFactory
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
  /// <summary>
  /// Create Candle with
  /// </summary>
  public Candle CreateCandle(DateTime? dt = null, bool initId = false)
  {
    var r = new Random();
    var open = r.Next(200, 500);
    var _ret = new MockCandle((dt ?? DateTime.Now).ToUniversalTime(),
      open,
      open + 100,
      open - 100,
      open + (r.Next(0, 100) - 50),
      r.Next(0, 100),
      this.instrument,
      this.timeFrame);
    if (initId)
      _ret.InitId(GetNextId());

    return _ret;
  }

  class MockCandle : Candle
  {
    public MockCandle(DateTime dateTime, int open, int high, int low, int close, int volume, [NotNull] ent.Instrument instrument, [NotNull] TimeFrame timeFrame) : base(dateTime, open, high, low, close, volume, instrument, timeFrame)
    {
    }
    public Candle InitId(int id)
    {
      Id = id;
      return this;
    }
  }

  /// <summary>
  /// Create new list of Candles with no ID
  /// </summary>
  public IEnumerable<Candle> CreateCandles(int count, DateTime? startDt = null, bool initId = false)
  {
    List<Candle> _ret_arr = new();
    var dt = (startDt ?? new DateTime(2020, 1, 1)).ToUniversalTime();
    for (int i = 0; i < count; i++)
    {
      _ret_arr.Add(CreateCandle(dt.AddDays(i), initId));
    }
    return _ret_arr;
  }

  /// <summary>
  /// Create new list of Candles with no ID
  /// </summary>
  /// <param name="fromDt"></param>
  /// <param name="untillDT"></param>
  /// <returns></returns>
  public IEnumerable<Candle> CreateCandles(DateTime fromDt, DateTime untillDT, bool initId = false, bool hourStep = false)
  {
    List<Candle> _ret_arr = new();
    var dt = fromDt.ToUniversalTime();
    while (dt < untillDT.ToUniversalTime())
    {
      _ret_arr.Add(CreateCandle(dt, initId));
      if (hourStep)
        dt = dt.AddHours(10);
      else
        dt = dt.AddDays(1);
    }
    return _ret_arr;
  }
}