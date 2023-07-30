using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.JoinedChartAggregate.Model;
public class JoinedChart_Factory_JoinTo_Period_Test : ExpectationsTestBase
{
  ent.Instrument usingInstrument;

  public JoinedChart_Factory_JoinTo_Period_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {
    usingInstrument = MockInstrument.Create(3, 3);

  }

  public static IEnumerable<object[]> TestCases
  {
    get
    {
      foreach (var isShuffled in new bool[] { true, false })
      {
        foreach (var periodShift in new int[] { 0, 1 })
        {
          foreach (var isCandleFromRep in new bool[] { true, false })
          {
            yield return new object[]{
            isShuffled, periodShift, isCandleFromRep
            };
          }
        }
      }
    }
  }

  [Theory]
  [MemberData(nameof(TestCases))]
  public void WHEN_give_candle_array_THEN_convert_correct(bool isShuffled, int periodShift, bool isCandleFromRep)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    //small shift to make first and last candle IsCalced false
    var usedChartFromDt = new DateTime(2020, 2, 4, 0, 0, 1).ToUniversalTime();
    var usedChartUntillDt = new DateTime(2020, 2, 6, 23, 59, 59).ToUniversalTime();

    var usedJoiningFromDt = usedChartFromDt + new TimeSpan(0, 4 * periodShift, 0, 0);
    var usedJoiningUntillDt = usedChartUntillDt - new TimeSpan(0, 8 * periodShift, 0, 0);

    var usedChart = new MockChartFactory(usingInstrument, new TimeFrame(TimeFrame.Enum.H4))
                            .CreateChart(usedChartFromDt, usedChartUntillDt, true);

    List<Candle> expectedCandles = MockCandles(usedChart);
    var candleRep = Substitute.For<IReadRepository<Candle>>();


    var usedCandles = expectedCandles.ToList();
    if (isShuffled)
    {
      Logger.LogDebug("Shuffle used data");
      var shuffledCandle = usedCandles[0];
      usedCandles.RemoveAt(0);
      usedCandles.Insert(usedCandles.Count() - 2, shuffledCandle);
    }
    if (isCandleFromRep)
      candleRep.Table.Returns(expectedCandles.BuildMock());
    else
    {
      var arrayResult = usedChart.AddCandles(usedCandles);
      Assert.True(arrayResult.IsSuccess, "Array wasn't succesfull");
    }

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedJoinedChart = JoinedChartManagerFunction.JoinTo(usedChart, TimeFrame.Enum.D1.ToEntity(), usedJoiningFromDt, usedJoiningUntillDt, candleRep);
    var assertedJoinedCandles = assertedJoinedChart.JoinedCandles!.ToArray();

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");
    ExpectGroup("Joined chart has correct data", () =>
    {
      Expect("From date is correct", () => Assert.Equal(usedChartFromDt, assertedJoinedChart.FromDate));
      Expect("Untill date is correct", () => Assert.Equal(usedChartUntillDt, assertedJoinedChart.UntillDate));
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
        new DateTime(2020, 2, 6,20,0,0).ToUniversalTime(),
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
    var candleRep = Substitute.For<IReadRepository<Candle>>();
    candleRep.Table.Returns(usedCandles.BuildMock());

    var arrayReslut = usedChart.AddCandles(usedCandles);
    Assert.True(arrayReslut.IsSuccess, "Array wasn't succesfull");
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get exception",
      () => Assert.Throws<ArgumentException>(() => JoinedChartManagerFunction.JoinTo(usedChart, usedFailEnum.ToEntity(), usedFromDt, usedUntillDt, candleRep)),
      out var assertedException);
    Expect("Parameter is TimeFrame", () => Assert.Equal("targetTimeFrame", assertedException.ParamName));

    #endregion
  }


  [Theory]
  [InlineData(true, true)]
  [InlineData(true, false)]
  [InlineData(false, true)]
  public void WHEN_give_from_or_untill_out_of_bounds_of_chart_THEN_exception(bool fromOut, bool untillOut)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var usedChartFromDt = new DateTime(2020, 2, 4, 0, 0, 1).ToUniversalTime();
    var usedChartUntillDt = new DateTime(2020, 2, 6, 23, 59, 59).ToUniversalTime();
    var usedChart = new MockChartFactory(usingInstrument, new TimeFrame(TimeFrame.Enum.H4))
                            .CreateChart(usedChartFromDt, usedChartUntillDt, true);
    List<Candle> usedCandles = MockCandles(usedChart);
    var candleRep = Substitute.For<IReadRepository<Candle>>();
    candleRep.Table.Returns(usedCandles.BuildMock());

    var arrayReslut = usedChart.AddCandles(usedCandles);
    Assert.True(arrayReslut.IsSuccess, "Array wasn't succesfull");

    var usedFromDt = usedChartFromDt;
    if (fromOut)
      usedFromDt = usedFromDt - new TimeSpan(1);

    var usedUntillDt = usedChartUntillDt;
    if (untillOut)
      usedUntillDt = usedUntillDt + new TimeSpan(1);

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get exception",
      () => Assert.Throws<ArgumentOutOfRangeException>(() => JoinedChartManagerFunction.JoinTo(usedChart, TimeFrame.Enum.D1.ToEntity(), usedFromDt, usedUntillDt, candleRep)),
      out var assertedException);
    Expect("Parameter is From or Untill", () => Assert.True("from" == assertedException.ParamName || "untill" == assertedException.ParamName));

    #endregion
  }
}