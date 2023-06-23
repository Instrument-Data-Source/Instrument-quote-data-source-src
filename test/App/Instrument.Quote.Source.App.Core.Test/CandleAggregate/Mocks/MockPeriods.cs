using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;

public class MockPeriodFactory
{
  private readonly ent.Instrument instrument;
  private readonly TimeFrame timeFrame;
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

  public LoadedPeriod CreateRandomPeriod(DateTime? fromDt = null, DateTime? untillDt = null)
  {
    var expectedFrom = (fromDt ?? new DateTime(2020, 1, 1)).ToUniversalTime();
    var expectedUntill = (untillDt ?? new DateTime(2020, 1, 10)).ToUniversalTime();
    var expectedCandles = candleFactory.CreateRandomCandles(expectedFrom, expectedUntill);

    return new LoadedPeriod(expectedFrom, expectedUntill, instrument, timeFrame, expectedCandles);
  }
  

}