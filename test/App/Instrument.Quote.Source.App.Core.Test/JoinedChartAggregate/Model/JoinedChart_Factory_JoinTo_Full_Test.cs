using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.JoinedChartAggregate.Model;
public class JoinedChart_Factory_JoinTo_Full_Test : ExpectationsTestBase
{
  ent.Instrument usingInstrument;

  public JoinedChart_Factory_JoinTo_Full_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {
    usingInstrument = MockInstrument.Create(3, 3);

  }

  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void WHEN_give_candle_array_THEN_convert_correct(bool isShuffled)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var usedFromDt = new DateTime(2020, 2, 4, 0, 0, 1).ToUniversalTime();
    var usedUntillDt = new DateTime(2020, 2, 6, 23, 59, 59).ToUniversalTime();
    var usedChart = new MockChartFactory(usingInstrument, new TimeFrame(TimeFrame.Enum.H4))
                            .CreateChart(usedFromDt, usedUntillDt, true);
    List<Candle> expectedCandles = MockCandles(usedChart);

    var usedCandles = expectedCandles.ToList();
    if (isShuffled)
    {
      Logger.LogDebug("Shuffle used data");
      var shuffledCandle = usedCandles[0];
      usedCandles.RemoveAt(0);
      usedCandles.Insert(usedCandles.Count() - 2, shuffledCandle);
    }

    var arrayReslut = usedChart.AddCandles(usedCandles);
    Assert.True(arrayReslut.IsSuccess, "Array wasn't succesfull");

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedJoinedChart = JoinedChartManagerFunction.JoinTo(usedChart, TimeFrame.Enum.D1.ToEntity());
    var assertedJoinedCandles = assertedJoinedChart.JoinedCandles.ToArray();

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");
    ExpectGroup("Joined chart has correct data", () =>
    {
      Expect("From date is correct", () => Assert.Equal(usedFromDt, assertedJoinedChart.FromDate));
      Expect("Untill date is correct", () => Assert.Equal(usedUntillDt, assertedJoinedChart.UntillDate));
      Expect("Target timefarme is correct", () => Assert.Equal(TimeFrame.Enum.D1, assertedJoinedChart.TargetTimeFrame.EnumId));
      Expect("Chart is correct", () => Assert.True(usedChart == assertedJoinedChart.StepChart));
    });

    ExpectGroup("Joined Candles is correct", () =>
    {
      int idx = 0;
      Logger.LogDebug("JoinedCandle 1");
      ExpectGroup($"Check candle {idx}", () =>
      {
        Assert.Equal(expectedCandles[idx].DateTime, assertedJoinedCandles[idx].StepDateTime);
        Assert.Equal(new DateTime(2020, 2, 4).ToUniversalTime(), assertedJoinedCandles[idx].TargetDateTime);
        Assert.Equal(100, assertedJoinedCandles[idx].Open);
        Assert.Equal(150, assertedJoinedCandles[idx].High);
        Assert.Equal(90, assertedJoinedCandles[idx].Low);
        Assert.Equal(110, assertedJoinedCandles[idx].Close);
        Assert.Equal(1000, assertedJoinedCandles[idx].Volume);
        Assert.Equal(false, assertedJoinedCandles[idx].IsLast);
        Assert.Equal(false, assertedJoinedCandles[idx].IsFullCalc);
      });
      idx++;
      ExpectGroup($"Check candle {idx}", () =>
      {
        Assert.Equal(expectedCandles[idx].DateTime, assertedJoinedCandles[idx].StepDateTime);
        Assert.Equal(new DateTime(2020, 2, 4).ToUniversalTime(), assertedJoinedCandles[idx].TargetDateTime);
        Assert.Equal(100, assertedJoinedCandles[idx].Open);
        Assert.Equal(170, assertedJoinedCandles[idx].High);
        Assert.Equal(90, assertedJoinedCandles[idx].Low);
        Assert.Equal(140, assertedJoinedCandles[idx].Close);
        Assert.Equal(2200, assertedJoinedCandles[idx].Volume);
        Assert.Equal(true, assertedJoinedCandles[idx].IsLast);
        Assert.Equal(false, assertedJoinedCandles[idx].IsFullCalc);
      });

      Logger.LogDebug("JoinedCandle 2");
      idx++;
      ExpectGroup($"Check candle {idx}", () =>
      {
        Assert.Equal(expectedCandles[idx].DateTime, assertedJoinedCandles[idx].StepDateTime);
        Assert.Equal(new DateTime(2020, 2, 5).ToUniversalTime(), assertedJoinedCandles[idx].TargetDateTime);
        Assert.Equal(100, assertedJoinedCandles[idx].Open);
        Assert.Equal(150, assertedJoinedCandles[idx].High);
        Assert.Equal(90, assertedJoinedCandles[idx].Low);
        Assert.Equal(110, assertedJoinedCandles[idx].Close);
        Assert.Equal(1000, assertedJoinedCandles[idx].Volume);
        Assert.Equal(false, assertedJoinedCandles[idx].IsLast);
        Assert.Equal(true, assertedJoinedCandles[idx].IsFullCalc);
      });
      idx++;
      ExpectGroup($"Check candle {idx}", () =>
      {
        Assert.Equal(expectedCandles[idx].DateTime, assertedJoinedCandles[idx].StepDateTime);
        Assert.Equal(new DateTime(2020, 2, 5).ToUniversalTime(), assertedJoinedCandles[idx].TargetDateTime);
        Assert.Equal(100, assertedJoinedCandles[idx].Open);
        Assert.Equal(170, assertedJoinedCandles[idx].High);
        Assert.Equal(90, assertedJoinedCandles[idx].Low);
        Assert.Equal(140, assertedJoinedCandles[idx].Close);
        Assert.Equal(2200, assertedJoinedCandles[idx].Volume);
        Assert.Equal(false, assertedJoinedCandles[idx].IsLast);
        Assert.Equal(true, assertedJoinedCandles[idx].IsFullCalc);
      });
      idx++;
      ExpectGroup($"Check candle {idx}", () =>
      {

        Assert.Equal(expectedCandles[idx].DateTime, assertedJoinedCandles[idx].StepDateTime);
        Assert.Equal(new DateTime(2020, 2, 5).ToUniversalTime(), assertedJoinedCandles[idx].TargetDateTime);
        Assert.Equal(100, assertedJoinedCandles[idx].Open);
        Assert.Equal(170, assertedJoinedCandles[idx].High);
        Assert.Equal(80, assertedJoinedCandles[idx].Low);
        Assert.Equal(100, assertedJoinedCandles[idx].Close);
        Assert.Equal(2400, assertedJoinedCandles[idx].Volume);
        Assert.Equal(true, assertedJoinedCandles[idx].IsLast);
        Assert.Equal(true, assertedJoinedCandles[idx].IsFullCalc);

      });

      Logger.LogDebug("JoinedCandle 3");
      idx++;
      ExpectGroup($"Check candle {idx}", () =>
      {

        Assert.Equal(expectedCandles[idx].DateTime, assertedJoinedCandles[idx].StepDateTime);
        Assert.Equal(new DateTime(2020, 2, 6).ToUniversalTime(), assertedJoinedCandles[idx].TargetDateTime);
        Assert.Equal(120, assertedJoinedCandles[idx].Open);
        Assert.Equal(120, assertedJoinedCandles[idx].High);
        Assert.Equal(80, assertedJoinedCandles[idx].Low);
        Assert.Equal(110, assertedJoinedCandles[idx].Close);
        Assert.Equal(100, assertedJoinedCandles[idx].Volume);
        Assert.Equal(false, assertedJoinedCandles[idx].IsLast);
        Assert.Equal(false, assertedJoinedCandles[idx].IsFullCalc);
      });
      idx++;
      ExpectGroup($"Check candle {idx}", () =>
      {

        Assert.Equal(expectedCandles[idx].DateTime, assertedJoinedCandles[idx].StepDateTime);
        Assert.Equal(new DateTime(2020, 2, 6).ToUniversalTime(), assertedJoinedCandles[idx].TargetDateTime);
        Assert.Equal(120, assertedJoinedCandles[idx].Open);
        Assert.Equal(120, assertedJoinedCandles[idx].High);
        Assert.Equal(80, assertedJoinedCandles[idx].Low);
        Assert.Equal(100, assertedJoinedCandles[idx].Close);
        Assert.Equal(100, assertedJoinedCandles[idx].Volume);
        Assert.Equal(true, assertedJoinedCandles[idx].IsLast);
        Assert.Equal(false, assertedJoinedCandles[idx].IsFullCalc);
      });

    });
    #endregion

  }

  private static List<Candle> MockCandles(MockChartFactory.MockChart usedChart)
  {
    return new List<Candle>(){
      new Candle(
        new DateTime(2020, 2, 4,4,0,0).ToUniversalTime(),
        100,
        150,90,
        110,
        1000,
        usedChart
      ),
      new Candle(
        new DateTime(2020, 2, 4,8,0,0).ToUniversalTime(),
        120,
        170,90,
        140,
        1200,
        usedChart
      ),
      new Candle(
        new DateTime(2020, 2, 5).ToUniversalTime(),
        100,
        150,90,
        110,
        1000,
        usedChart
      ),
      new Candle(
        new DateTime(2020, 2, 5,4,0,0).ToUniversalTime(),
        120,
        170,90,
        140,
        1200,
        usedChart
      ),
      new Candle(
        new DateTime(2020, 2, 5,8,0,0).ToUniversalTime(),
        120,
        130,80,
        100,
        200,
        usedChart
      ),
      new Candle(
        new DateTime(2020, 2, 6).ToUniversalTime(),
        120,
        120,80,
        110,
        100,
        usedChart
      ),
      new Candle(
        new DateTime(2020, 2, 6,4,0,0).ToUniversalTime(),
        100,
        100,100,
        100,
        0,
        usedChart
      )
    };
  }

  [Theory]
  [InlineData(TimeFrame.Enum.H1)]
  [InlineData(TimeFrame.Enum.H4)]
  public void WHEN_give_timeframe_is_LE_than_chart_timeframe_THEN_exception(TimeFrame.Enum usedFailEnum)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var usedFromDt = new DateTime(2020, 2, 4, 0, 0, 1).ToUniversalTime();
    var usedUntillDt = new DateTime(2020, 2, 6, 23, 59, 59).ToUniversalTime();
    var usedChart = new MockChartFactory(usingInstrument, new TimeFrame(TimeFrame.Enum.H4))
                            .CreateChart(usedFromDt, usedUntillDt, true);
    List<Candle> usedCandles = MockCandles(usedChart);
    var arrayReslut = usedChart.AddCandles(usedCandles);
    Assert.True(arrayReslut.IsSuccess, "Array wasn't succesfull");
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get exception",
      () => Assert.Throws<ArgumentException>(() => JoinedChartManagerFunction.JoinTo(usedChart, usedFailEnum.ToEntity())),
      out var assertedException);
    Expect("Parameter is TimeFrame", () => Assert.Equal("targetTimeFrame", assertedException.ParamName));

    #endregion
  }

  [Fact]
  public void WHEN_candles_did_not_loaded_that_error_THEN_exception()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var usedFromDt = new DateTime(2020, 2, 4, 0, 0, 1).ToUniversalTime();
    var usedUntillDt = new DateTime(2020, 2, 6, 23, 59, 59).ToUniversalTime();
    var usedChart = new MockChartFactory(usingInstrument, new TimeFrame(TimeFrame.Enum.H4))
                            .CreateChart(usedFromDt, usedUntillDt, true);
    List<Candle> usedCandles = MockCandles(usedChart);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get exception",
      () => Assert.Throws<ArgumentNullException>(() => JoinedChartManagerFunction.JoinTo(usedChart, TimeFrame.Enum.D1.ToEntity())),
      out var assertedException);
    //Expect("Parameter is TimeFrame", () => Assert.Equal("Candles", assertedException.ParamName));

    #endregion
  }
}