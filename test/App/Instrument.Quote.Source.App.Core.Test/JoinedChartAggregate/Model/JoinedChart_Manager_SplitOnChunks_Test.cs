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

public class JoinedChart_Manager_SplitOnChunks_Test : ExpectationsTestBase
{

  public JoinedChart_Manager_SplitOnChunks_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {

  }

  [Fact]
  public void WHEN_split_new_period_of_base_chart_THEN_it_splitted()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var chartFrom = new DateTime(2020, 1, 4).ToUniversalTime();
    var chartUntill = new DateTime(2020, 6, 15).ToUniversalTime();
    Chart baseChart = new MockChartFactory(TimeFrame.Enum.D1).CreateChart(chartFrom, chartUntill, true);
    var joinedChartFrom = new DateTime(2020, 3, 1).ToUniversalTime();
    var joinedChartUntill = new DateTime(2020, 4, 1).ToUniversalTime();
    var targetTimeFrame = TimeFrame.Enum.M;
    JoinedChart joinedChart = new JoinedChart(joinedChartFrom, joinedChartUntill, baseChart, targetTimeFrame.ToEntity());

    Logger.LogDebug("Prepare expetcted chunks");
    /*
    Chunks must be order from closest to exist joinedChart to the fare
    */
    var expectedChunks = new List<DateTimePeriod>(){
      new DateTimePeriod(new DateTime(2020, 2, 1).ToUniversalTime(),new DateTime(2020, 3, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 1, 4).ToUniversalTime(),new DateTime(2020, 2, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 4, 1).ToUniversalTime(),new DateTime(2020, 5, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 5, 1).ToUniversalTime(),new DateTime(2020, 6, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 6, 1).ToUniversalTime(),new DateTime(2020, 6, 15).ToUniversalTime()),
    };

    #endregion

    #region Act
    Logger.LogDebug("Test ACT");

    var assertedChunks = JoinedChart.Manager.SplitOnChunks(baseChart, joinedChart);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get correct chunk Count", () => Assert.Equal(expectedChunks.Count(), assertedChunks.Count()));
    Expect("Chunnks returns in correct order, from exist data to baseChart border", () =>
    {
      for (int i = 0; i < expectedChunks.Count(); i++)
      {
        Assert.Equal(expectedChunks[i], assertedChunks.ElementAt(i));
      }
    });

    #endregion
  }

  [Fact]
  public void WHEN_split_new_period_and_max_count_can_fit_more_than_1_target_perion_THEN_condense_several_target_periods()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var chartFrom = new DateTime(2020, 1, 4).ToUniversalTime();
    var chartUntill = new DateTime(2020, 6, 15).ToUniversalTime();
    Chart baseChart = new MockChartFactory(TimeFrame.Enum.D1).CreateChart(chartFrom, chartUntill, true);
    var joinedChartFrom = new DateTime(2020, 3, 1).ToUniversalTime();
    var joinedChartUntill = new DateTime(2020, 4, 1).ToUniversalTime();
    var targetTimeFrame = TimeFrame.Enum.M;
    JoinedChart joinedChart = new JoinedChart(joinedChartFrom, joinedChartUntill, baseChart, targetTimeFrame.ToEntity());

    Logger.LogDebug("Prepare expetcted chunks");
    /*
    Chunks must be order from closest to exist joinedChart to the fare
    */
    var expectedChunks = new List<DateTimePeriod>(){
      new DateTimePeriod(new DateTime(2020, 1, 4).ToUniversalTime(),new DateTime(2020, 3, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 4, 1).ToUniversalTime(),new DateTime(2020, 6, 1).ToUniversalTime()),
      new DateTimePeriod(new DateTime(2020, 6, 1).ToUniversalTime(),new DateTime(2020, 6, 15).ToUniversalTime()),
    };

    #endregion

    #region Act
    Logger.LogDebug("Test ACT");

    var assertedChunks = JoinedChart.Manager.SplitOnChunks(baseChart, joinedChart, 64);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get correct chunk Count", () => Assert.Equal(expectedChunks.Count(), assertedChunks.Count()));
    Expect("Chunnks returns in correct order, from exist data to baseChart border", () =>
    {
      for (int i = 0; i < expectedChunks.Count(); i++)
      {
        Assert.Equal(expectedChunks[i], assertedChunks.ElementAt(i));
      }
    });

    #endregion
  }
}