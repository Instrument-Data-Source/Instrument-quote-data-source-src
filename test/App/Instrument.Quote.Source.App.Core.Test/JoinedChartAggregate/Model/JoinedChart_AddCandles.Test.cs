using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Tools;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
/*
namespace Instrument.Quote.Source.App.Core.Test.JoinedChartAggregate.Model;

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
    var usedTillDt = new DateTime(2021, 1, 1).ToUniversalTime();
    var usedJoinedChart = new JoinedChart(usedFromDt, usedTillDt, usedChart, TimeFrame.Enum.M.ToEntity());

    var usedCandles = mockCandleFactory.CreateCandles(usedFromDt, usedTillDt);
    var mapper = new JoinedCandleMapper(usedJoinedChart);
    var usedJoinedCandle = usedCandles.Select(mapper.map).ToArray()
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = usedJoinedChart.AddCandles(usedCandles);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Chart contain candles", () =>
    {
      Expect("Count of candles correct", () => Assert.Equal(usedCandles.Count(), usedJoinedChart.JoinedCandles.Count()));
      Expect("Chart contain each candles", () => Assert.True(expectedCandles.All(c => assertedChart.Candles.Contains(c))));
    });

    #endregion
  }

  [Fact]
  public void WHEN_add_candles_to_filled_chart_THEN_add_new_candles()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    throw new NotImplementedException();

    #endregion
  }
}
*/