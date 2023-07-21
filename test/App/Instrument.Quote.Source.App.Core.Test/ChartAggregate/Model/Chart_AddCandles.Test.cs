using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Microsoft.Extensions.Logging;
using Ardalis.Result;
using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Model;

public class Chart_AddCandles_Test : ExpectationsTestBase
{
  MockChartFactory mockChartFactory = new MockChartFactory();
  Chart assertedChart;
  //IServiceProvider sp = Substitute.For<IServiceProvider>();
  //IReadRepository<ent.Instrument> instrumentRep = Substitute.For<IReadRepository<ent.Instrument>>();
  public Chart_AddCandles_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {
    assertedChart = mockChartFactory.CreateChart();
    //sp.GetService(Arg.Is(typeof(IReadRepository<ent.Instrument>))).Returns(instrumentRep);
    //sp.GetService(Arg.Is(typeof(IDecimalPartLongCheckerFactory))).Returns(new DecimalToStoreIntConverterFactory());
    //instrumentRep.Table.Returns(new[] { mockChartFactory.instrument }.BuildMock());

  }

  [Fact]
  public void WHEN_add_new_candle_THEN_candles_add_correctly()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var expectedCandles = new MockCandleFactory(assertedChart).CreateCandles(assertedChart.FromDate, assertedChart.UntillDate).ToList();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    assertedChart.AddCandles(expectedCandles);

    #endregion

    #region Assert
    Logger.LogDebug("Test ASSERT");

    ExpectGroup("Chart contain candles", () =>
    {
      Expect("Count of candles correct", () => Assert.Equal(expectedCandles.Count(), assertedChart.Candles.Count()));
      Expect("Chart contain each candles", () => Assert.True(expectedCandles.All(c => assertedChart.Candles.Contains(c))));
    });
    #endregion
  }

  [Fact]
  public void WHEN_adding_new_candle_to_exist_data_THEN_add_new_data()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var midDateTime = assertedChart.UntillDate - new TimeSpan(4, 0, 0, 0);
    var usedCandles = new MockCandleFactory(assertedChart).CreateCandles(assertedChart.FromDate, midDateTime).ToList();
    assertedChart.AddCandles(usedCandles);

    var expectedCandles = new MockCandleFactory(assertedChart).CreateCandles(midDateTime, assertedChart.UntillDate).ToList();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    assertedChart.AddCandles(expectedCandles);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    ExpectGroup("Chart contain candles", () =>
    {
      Expect("Count of candles correct", () => Assert.Equal(usedCandles.Count() + expectedCandles.Count(), assertedChart.Candles.Count()));
      Expect("Chart contain each base candles", () => Assert.True(usedCandles.All(c => assertedChart.Candles.Contains(c))));
      Expect("Chart contain each new candles", () => Assert.True(expectedCandles.All(c => assertedChart.Candles.Contains(c))));
    });


    #endregion
  }

  public static IEnumerable<object[]> WrongShift
  {
    get
    {

      yield return new object[] { -3, -2 };
      yield return new object[] { -3, 0 };
      yield return new object[] { -3, +2 };
      yield return new object[] { 0, +2 };
      yield return new object[] { +2, +2 };
    }
  }
  [Theory]
  [MemberData(nameof(WrongShift))]
  public void WHEN_candles_shift_of_period_THEN_error(int shiftFrom, int shiftUntill)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var usedFromDt = assertedChart.FromDate + new TimeSpan(shiftFrom, 0, 0, 0);
    var usedUntillDt = assertedChart.UntillDate + new TimeSpan(shiftUntill, 0, 0, 0);
    var usedCandles = new MockCandleFactory(assertedChart).CreateCandles(usedFromDt, usedUntillDt);

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = assertedChart.AddCandles(usedCandles);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Successfull", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status not valid", () => Assert.Equal(ResultStatus.Invalid, assertedResult.Status));
    #endregion
  }
  [Theory]
  [InlineData(true)]
  [InlineData(false)]
  public void WHEN_candles_out_of_period_THEN_error(bool rightSide)
  {
    Logger.LogInformation($"{nameof(rightSide)}: {rightSide}");
    #region Array
    Logger.LogDebug("Test ARRAY");
    DateTime usedFromDt = assertedChart.UntillDate;
    DateTime usedUntillDt = assertedChart.FromDate;
    if (rightSide)
      usedUntillDt = usedFromDt + new TimeSpan(1, 0, 0, 0);
    else
      usedFromDt = usedUntillDt - new TimeSpan(1, 0, 0, 0);

    var usedCandles = new MockCandleFactory(assertedChart).CreateCandles(usedFromDt, usedUntillDt);

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = assertedChart.AddCandles(usedCandles);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Successfull", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status not valid", () => Assert.Equal(ResultStatus.Invalid, assertedResult.Status));
    ExpectGroup("Errors is correct", () =>
    {
      Expect("Errors count 1", () => Assert.Single(assertedResult.ValidationErrors), out var assertedError);
      Expect("Field DateTime", () => Assert.True(assertedError.Identifier.Contains("DateTime")));
    });
    #endregion
  }

  [Fact]
  public void WHEN_candles_does_not_fit_timeframe_THEN_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var usedCandlesDtos = new MockCandleFactory(assertedChart).CreateCandleDtos(assertedChart.FromDate, assertedChart.UntillDate).ToArray();
    usedCandlesDtos[2].DateTime += new TimeSpan(1, 0, 0);
    var usedMapper = new CandleMapper(assertedChart);
    var usedCandles = usedCandlesDtos.Select(usedMapper.map);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = assertedChart.AddCandles(usedCandles);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Successfull", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status not valid", () => Assert.Equal(ResultStatus.Invalid, assertedResult.Status));
    ExpectGroup("Errors is correct", () =>
    {
      Expect("Errors count 1", () => Assert.Single(assertedResult.ValidationErrors), out var assertedError);
      Expect("Field DateTime", () => Assert.True(assertedError.Identifier.Contains("DateTime")));
    });

    #endregion
  }



  [Fact]
  public void WHEN_candles_duplicate_with_exist_data_THEN_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var midDateTime = assertedChart.UntillDate - new TimeSpan(10, 0, 0, 0);
    var expectedCandles = new MockCandleFactory(assertedChart).CreateCandles(assertedChart.FromDate, midDateTime).ToList();
    assertedChart.AddCandles(expectedCandles);

    var usedCandles = new MockCandleFactory(assertedChart).CreateCandles(midDateTime, assertedChart.UntillDate).ToList();
    usedCandles.Add(expectedCandles[2]);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = assertedChart.AddCandles(usedCandles);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Successfull", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status not valid", () => Assert.Equal(ResultStatus.Invalid, assertedResult.Status));
    ExpectGroup("Errors is correct", () =>
    {
      Expect("Errors count 1", () => Assert.Single(assertedResult.ValidationErrors), out var assertedError);
      Expect("Field DateTime", () => Assert.True(assertedError.Identifier.Contains("DateTime")));
    });


    ExpectGroup("Exist candles doesn't changed", () =>
    {
      Expect("Count of candles correct", () => Assert.Equal(expectedCandles.Count(), assertedChart.Candles.Count()));
      Expect("Chart contain each new candles", () => Assert.True(expectedCandles.All(c => assertedChart.Candles.Contains(c))));
    });
    #endregion
  }

  [Fact]
  public void WHEN_candles_duplicate_between_each_other_THEN_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var usedCandles = new MockCandleFactory(assertedChart)
        .CreateCandles(assertedChart.FromDate, assertedChart.UntillDate).ToList();
    usedCandles.Add(usedCandles[2]);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = assertedChart.AddCandles(usedCandles);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Successfull", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status not valid", () => Assert.Equal(ResultStatus.Invalid, assertedResult.Status));
    ExpectGroup("Errors is correct", () =>
    {
      Expect("Errors count 1", () => Assert.Single(assertedResult.ValidationErrors), out var assertedError);
      Expect("Field DateTime", () => Assert.True(assertedError.Identifier.Contains("DateTime")));
    });

    #endregion
  }
}