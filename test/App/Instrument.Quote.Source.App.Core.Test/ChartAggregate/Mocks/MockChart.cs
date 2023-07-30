using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;

public class MockChartFactory : absMockFactory
{
  public ent.Instrument instrument;
  public TimeFrame timeFrame;
  public MockChartFactory(ent.Instrument instrument, TimeFrame timeFrame)
  {
    this.instrument = instrument;
    this.timeFrame = timeFrame;
  }

  public MockChartFactory() : this(MockInstrument.Create(), TimeFrame.Enum.D1.ToEntity())
  {

  }
  public MockChartFactory(TimeFrame.Enum timeFrameEnum) : this(MockInstrument.Create(), timeFrameEnum.ToEntity())
  {

  }
  public MockChartFactory SetInstrument(ent.Instrument? newInstrument = null)
  {
    if (newInstrument == null)
      newInstrument = MockInstrument.Create();
    return new MockChartFactory(newInstrument, this.timeFrame);
  }
  public MockChartFactory SetTimeFrame(TimeFrame newTimeFrame)
  {
    return new MockChartFactory(this.instrument, newTimeFrame);
  }

  private static HashSet<int> usedId = new HashSet<int>();
  /// <summary>
  /// Create new periods
  /// </summary>
  /// <param name="fromDt"></param>
  /// <param name="untillDt"></param>
  /// <returns></returns>
  public MockChart CreateChart(DateTime? fromDt = null, DateTime? untillDt = null, bool initId = false)
  {
    var expectedFrom = (fromDt ?? new DateTime(2020, 1, 1)).ToUniversalTime();
    var expectedUntill = (untillDt ?? new DateTime(2020, 1, 20)).ToUniversalTime();

    var _ret = new MockChart(expectedFrom, expectedUntill, instrument, timeFrame);
    
    if (initId)
      _ret.InitId(GetNextId());

    return _ret;
  }

  public class MockChart : Chart
  {
    public MockChart(DateTime from, DateTime untill, ent.Instrument instrument, TimeFrame timeFrame)
                          : base(from, untill, instrument, timeFrame)
    {
    }

    public Chart InitId(int id)
    {
      this.Id = id;
      return this;
    }
  }
}