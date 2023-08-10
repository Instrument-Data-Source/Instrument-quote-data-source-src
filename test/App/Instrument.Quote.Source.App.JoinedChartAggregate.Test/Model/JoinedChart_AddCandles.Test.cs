using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Service;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Test.Model;
/*
public class JoinedChart_AddCandles_Test : ExpectationsTestBase
{
  MockChartFactory mockChartFactory = new MockChartFactory();
  MockCandleFactory mockCandleFactory;
  Chart usedChart;
  public JoinedChart_AddCandles_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {
    usedChart = mockChartFactory.CreateChart();
    mockCandleFactory = new MockCandleFactory(usedChart);
  }

  [Fact]
  public void WHEN_add_candles_to_empty_chart_THEN_add_new_candles()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var usedFromDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var usedMidlDt = new DateTime(2020, 7, 1).ToUniversalTime();
    var usedTillDt = new DateTime(2021, 1, 1).ToUniversalTime();
    var usedJoinedChart = new JoinedChart(usedFromDt, usedTillDt, usedChart, TimeFrame.Enum.M.ToEntity());

    var usedCandles = mockCandleFactory.CreateCandles(usedFromDt, usedTillDt);
    var usedCandles1 = usedCandles.Where(c => c.DateTime >= usedFromDt && c.DateTime < usedMidlDt).ToList();
    var usedCandles2 = usedCandles.Where(c => c.DateTime >= usedMidlDt && c.DateTime < usedTillDt).ToList();
    var mapper = new JoinedCandleMapper(usedJoinedChart);
    var usedJoinedCandle = usedCandles.Select(mapper.map).ToArray();
    var usedJoinedCandle1 = usedCandles1.Select(mapper.map).ToArray();
    var usedJoinedCandle2 = usedCandles2.Select(mapper.map).ToArray();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = usedJoinedChart.AddCandles(usedCandles1);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Chart contain candles", () =>
    {
      Expect("Count of candles correct", () => Assert.Equal(usedCandles1.Count(), usedJoinedChart.JoinedCandles.Count()));
      Expect("Chart contain each candles blokc 1", () => Assert.True(usedCandles21.All(c => usedJoinedChart.JoinedCandles.Contains(c))));
    });

    #endregion

    #region Act 2
    Logger.LogDebug("Test ACT 2");

    var assertedResult = usedJoinedChart.AddCandles(usedCandles2);

    #endregion


    #region Assert 2
    Logger.LogDebug("Test ASSERT 2");

    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Chart contain candles", () =>
    {
      Expect("Count of candles correct", () => Assert.Equal(usedCandles2.Count(), usedJoinedChart.JoinedCandles.Count()));
      Expect("Chart contain each candles block 2", () => Assert.True(usedCandles2.All(c => usedJoinedChart.JoinedCandles.Contains(c))));
      Expect("Chart contain each candles", () => Assert.True(usedCandles2.All(c => usedJoinedChart.JoinedCandles.Contains(c))));
    });

    #endregion
  }

}
*/