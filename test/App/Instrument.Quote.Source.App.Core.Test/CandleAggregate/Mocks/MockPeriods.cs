using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;

public class MockPeriodFactory : absMockFactory
{
  public ent.Instrument instrument;
  public TimeFrame timeFrame;
  private MockCandleFactory candleFactory;
  public MockPeriodFactory(ent.Instrument instrument, TimeFrame timeFrame)
  {
    this.instrument = instrument;
    this.timeFrame = timeFrame;
    candleFactory = new MockCandleFactory(instrument, timeFrame);
  }

  public MockPeriodFactory() : this(MockInstrument.Create(), TimeFrame.Enum.D1.ToEntity())
  {

  }
  public MockPeriodFactory(TimeFrame.Enum timeFrameEnum) : this(MockInstrument.Create(), timeFrameEnum.ToEntity())
  {

  }
  public MockPeriodFactory SetInstrument(ent.Instrument? newInstrument = null)
  {
    if (newInstrument == null)
      newInstrument = MockInstrument.Create();
    return new MockPeriodFactory(newInstrument, this.timeFrame);
  }
  public MockPeriodFactory SetTimeFrame(TimeFrame newTimeFrame)
  {
    return new MockPeriodFactory(this.instrument, newTimeFrame);
  }

  private static HashSet<int> usedId = new HashSet<int>();
  /// <summary>
  /// Create new periods
  /// </summary>
  /// <param name="fromDt"></param>
  /// <param name="untillDt"></param>
  /// <returns></returns>
  public LoadedPeriod CreatePeriod(DateTime? fromDt = null, DateTime? untillDt = null, bool initId = false)
  {
    var expectedFrom = (fromDt ?? new DateTime(2020, 1, 1)).ToUniversalTime();
    var expectedUntill = (untillDt ?? new DateTime(2020, 1, 10)).ToUniversalTime();

    var expectedCandles = candleFactory.CreateCandles(expectedFrom, expectedUntill, initId, timeFrame.EnumId != TimeFrame.Enum.D1);

    var _ret = new MockLoadedPeriod(expectedFrom, expectedUntill, instrument, timeFrame, expectedCandles);
    if (initId)
      _ret.InitId(GetNextId());

    return _ret;
  }

  public class MockLoadedPeriod : LoadedPeriod
  {
    public MockLoadedPeriod(DateTime from, DateTime untill, ent.Instrument instrument, TimeFrame timeFrame, IEnumerable<Candle> candles)
                          : base(from, untill, instrument, timeFrame, candles)
    {
    }

    public LoadedPeriod InitId(int id)
    {
      this.Id = id;
      return this;
    }
  }
}



