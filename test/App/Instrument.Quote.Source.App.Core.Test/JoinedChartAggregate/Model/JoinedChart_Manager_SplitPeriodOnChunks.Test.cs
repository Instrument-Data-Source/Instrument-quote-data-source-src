using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.DateTimePeriod;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using static Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks.MockChartFactory;

namespace Instrument.Quote.Source.App.Core.Test.JoinedChartAggregate.Model;

public class JoinedChart_Manager_SplitOnChunks_Period_Test : ExpectationsTestBase
{

  public JoinedChart_Manager_SplitOnChunks_Period_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {

  }

  [Fact]
  public void WHEN_give_interval_THEN_it_splitted_correctly()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var from = new DateTime(2020, 1, 4).ToUniversalTime();
    var untill = new DateTime(2020, 3, 10).ToUniversalTime();
    var usedPeriod = new DateTimePeriod(from, untill);
    var baseTimeFrame = TimeFrame.Enum.D1;
    var targetTimeFrame = TimeFrame.Enum.M;
    Logger.LogDebug("Prepare expetcted chunks");
    /*
    Chunks must be order from closest to exist joinedChart to the fare
    */
    var expectedChunks = new List<DateTimePeriod>(){
      new DateTimePeriod(new DateTime(2020, 1, 4).ToUniversalTime(),new DateTime(2020, 2, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 2, 1).ToUniversalTime(),new DateTime(2020, 3, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 3, 1).ToUniversalTime(),new DateTime(2020, 3, 10).ToUniversalTime()),
    };

    #endregion

    #region Act
    Logger.LogDebug("Test ACT");

    var assertedChunks = JoinedChart.Manager.SplitPeriodOnChunks(usedPeriod, baseTimeFrame, targetTimeFrame);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get correct chunk Count", () => Assert.Equal(expectedChunks.Count(), assertedChunks.Count()));
    Expect("Chunnks returns in correct order, from exist data to baseChart border", () =>
    {
      for (int i = 0; i < expectedChunks.Count(); i++)
      {
        Assert.Equal(expectedChunks[i].From, assertedChunks.ElementAt(i).From);
        Assert.Equal(expectedChunks[i].Untill, assertedChunks.ElementAt(i).Untill);
      }
    });

    #endregion
  }

  [Fact]
  public void WHEN_give_empty_interval_THEN_return_empty_chunk()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var from = new DateTime(2020, 1, 4).ToUniversalTime();
    var untill = new DateTime(2020, 1, 4).ToUniversalTime();
    var usedPeriod = new DateTimePeriod(from, untill);
    var baseTimeFrame = TimeFrame.Enum.D1;
    var targetTimeFrame = TimeFrame.Enum.M;
    Logger.LogDebug("Prepare expetcted chunks");
    /*
    Chunks must be order from closest to exist joinedChart to the fare
    */

    #endregion

    #region Act
    Logger.LogDebug("Test ACT");

    var assertedChunks = JoinedChart.Manager.SplitPeriodOnChunks(usedPeriod, baseTimeFrame, targetTimeFrame);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get correct chunk Count", () => Assert.Equal(0, assertedChunks.Count()));

    #endregion
  }

  [Fact]
  public void WHEN_give_interval_and_chunk_can_contain_several_target_timeframe_THEN_it_splitted_correctly()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var from = new DateTime(2020, 1, 4).ToUniversalTime();
    var untill = new DateTime(2020, 6, 10).ToUniversalTime();
    var usedPeriod = new DateTimePeriod(from, untill);
    var baseTimeFrame = TimeFrame.Enum.D1;
    var targetTimeFrame = TimeFrame.Enum.M;
    Logger.LogDebug("Prepare expetcted chunks");
    /*
    Chunks must be order from closest to exist joinedChart to the fare
    */
    var expectedChunks = new List<DateTimePeriod>(){
      new DateTimePeriod(new DateTime(2020, 1, 4).ToUniversalTime(),new DateTime(2020, 3, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 3, 1).ToUniversalTime(),new DateTime(2020, 5, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 5, 1).ToUniversalTime(),new DateTime(2020, 6, 10).ToUniversalTime()),
    };

    #endregion

    #region Act
    Logger.LogDebug("Test ACT");

    var assertedChunks = JoinedChart.Manager.SplitPeriodOnChunks(usedPeriod, baseTimeFrame, targetTimeFrame, 64);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get correct chunk Count", () => Assert.Equal(expectedChunks.Count(), assertedChunks.Count()));
    Expect("Chunnks returns in correct order, from exist data to baseChart border", () =>
    {
      for (int i = 0; i < expectedChunks.Count(); i++)
      {
        Assert.Equal(expectedChunks[i].From, assertedChunks.ElementAt(i).From);
        Assert.Equal(expectedChunks[i].Untill, assertedChunks.ElementAt(i).Untill);
      }
    });

    #endregion
  }
}