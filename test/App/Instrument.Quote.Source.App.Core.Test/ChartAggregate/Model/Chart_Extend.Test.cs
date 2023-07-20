using InsonusK.Xunit.ExpectationsTest;
using NSubstitute;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Ardalis.Result;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using MockQueryable.Moq;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Model;


public class Chart_Extend_Test : ExpectationsTestBase
{
  ent.Instrument mockInstrument;
  TimeFrame usedTimeFrame;
  Chart assertedChart;
  IEnumerable<Candle> baseCandles;
  MockChartFactory mockChartFactory = new MockChartFactory();
  MockCandleFactory mockCandleFactory;
  //IServiceProvider sp = Substitute.For<IServiceProvider>();
  //IReadRepository<ent.Instrument> instrumentRep = Substitute.For<IReadRepository<ent.Instrument>>();

  //MockChartFactory mockChartFactory2 = new MockChartFactory();

  public Chart_Extend_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {
    mockInstrument = mockChartFactory.instrument;
    usedTimeFrame = mockChartFactory.timeFrame;

    assertedChart = mockChartFactory.CreateChart(initId: true);

    //sp.GetService(Arg.Is(typeof(IReadRepository<ent.Instrument>))).Returns(instrumentRep);
    //sp.GetService(Arg.Is(typeof(IDecimalPartLongCheckerFactory))).Returns(new DecimalToStoreIntConverterFactory());
    //instrumentRep.Table.Returns(new[] { mockChartFactory.instrument, mockChartFactory2.instrument }.BuildMock());
    //assertedChart.SetServiceProvider(sp);

    mockCandleFactory = new MockCandleFactory(assertedChart);
    baseCandles = mockCandleFactory.CreateCandles(assertedChart.FromDate, assertedChart.UntillDate);
    assertedChart.AddCandles(baseCandles);
  }

  [Theory]
  [InlineData("left")]
  [InlineData("right")]
  public void WHEN_period_connected_THEN_update_period(string crossing_type)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    DateTime fromDt;
    DateTime untillDt;
    DateTime expectedFromDt;
    DateTime expectedUntillDt;
    switch (crossing_type)
    {
      case "left":
        fromDt = assertedChart.FromDate.AddDays(-10);
        untillDt = assertedChart.FromDate;
        expectedFromDt = fromDt;
        expectedUntillDt = assertedChart.UntillDate;
        break;
      case "right":
        fromDt = assertedChart.UntillDate;
        untillDt = assertedChart.UntillDate.AddDays(10);
        expectedFromDt = assertedChart.FromDate;
        expectedUntillDt = untillDt;
        break;
      default:
        throw new NotImplementedException();
    }
    var newChart = mockChartFactory.CreateChart(fromDt, untillDt, false);
    var expectedCandles = mockCandleFactory.CreateCandles(newChart.FromDate, newChart.UntillDate);
    //newChart.SetServiceProvider(sp);
    newChart.AddCandles(expectedCandles);

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = assertedChart.Extend(newChart);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");
    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));

    var assertedResultValue = assertedResult.Value;
    Expect("Return count of added candles", () => Assert.Equal(expectedCandles.Count(), assertedResultValue));

    ExpectGroup("New period has extended dates", () =>
    {
      Expect("From date is extend correctly", () => Assert.Equal(expectedFromDt, assertedChart.FromDate));
      Expect("Untill date is extend correctly", () => Assert.Equal(expectedUntillDt, assertedChart.UntillDate));
    });


    ExpectGroup("Candles exptend correctly", () =>
    {
      Expect("Count is summed", () => Assert.Equal(baseCandles.Count() + expectedCandles.Count(), assertedChart.Candles.Count()));
      Expect("Candles contain base candles", () => Assert.True(baseCandles.All(c => assertedChart.Candles.Contains(c))));
      Expect("Candles contain new candles", () => Assert.True(expectedCandles.All(c => assertedChart.Candles.Contains(c))));
    });
    #endregion
  }

  [Fact]
  public void WHEN_another_instrument_id_THEN_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var newChart = new MockChartFactory().CreateChart(assertedChart.FromDate.AddDays(-10), assertedChart.FromDate);
    var newCandles = mockCandleFactory.CreateCandles(newChart.FromDate, newChart.UntillDate);
    //newChart.SetServiceProvider(sp);
    newChart.AddCandles(newCandles);

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get expection", () => Assert.Throws<ArgumentException>(() => assertedChart.Extend(newChart)), out var assertedException);

    Expect("Parameter in exception is InstrumentId", () => Assert.Equal(nameof(Chart.InstrumentId), assertedException.ParamName));

    ExpectGroup("Candles is not damaged", () =>
    {
      Expect("Count is correct", () => Assert.Equal(baseCandles.Count(), assertedChart.Candles.Count()));
      Expect("Candles contain base candles", () => Assert.True(baseCandles.All(c => assertedChart.Candles.Contains(c))));
    });

    #endregion
  }

  [Fact]
  public void WHEN_another_timeframe_id_THEN_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var usedMockChartFactory = new MockChartFactory(mockInstrument, TimeFrame.Enum.H1.ToEntity());
    var newChart = usedMockChartFactory.CreateChart(assertedChart.FromDate.AddDays(-10), assertedChart.FromDate);
    var newCandles = mockCandleFactory.CreateCandles(newChart.FromDate, newChart.UntillDate);
    //newChart.SetServiceProvider(sp);
    newChart.AddCandles(newCandles);

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get expection", () => Assert.Throws<ArgumentException>(() => assertedChart.Extend(newChart)), out var assertedException);

    Expect("Parameter in exception is TimeFrameId", () => Assert.Equal(nameof(Chart.TimeFrameId), assertedException.ParamName));

    ExpectGroup("Candles is not damaged", () =>
    {
      Expect("Count is correct", () => Assert.Equal(baseCandles.Count(), assertedChart.Candles.Count()));
      Expect("Candles contain base candles", () => Assert.True(baseCandles.All(c => assertedChart.Candles.Contains(c))));
    });

    #endregion
  }

  [Fact]
  public void WHEN_extanded_period_has_id_THEN_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var newChart = mockChartFactory.CreateChart(assertedChart.FromDate.AddDays(-10), assertedChart.FromDate, initId: true);
    var newCandles = mockCandleFactory.CreateCandles(newChart.FromDate, newChart.UntillDate);
    //newChart.SetServiceProvider(sp);
    newChart.AddCandles(newCandles);

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Get expection", () => Assert.Throws<ArgumentException>(() => assertedChart.Extend(newChart)), out var assertedException);

    Expect("Parameter in exception is Id", () => Assert.Equal(nameof(Chart.Id), assertedException.ParamName));

    ExpectGroup("Candles is not damaged", () =>
    {
      Expect("Count is correct", () => Assert.Equal(baseCandles.Count(), assertedChart.Candles.Count()));
      Expect("Candles contain base candles", () => Assert.True(baseCandles.All(c => assertedChart.Candles.Contains(c))));
    });
    #endregion
  }

  [Theory]
  [InlineData("left")]
  [InlineData("right")]
  [InlineData("inside")]
  [InlineData("same")]
  [InlineData("over")]
  public void WHEN_dates_are_crossing_THEN_error(string crossing_type)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    DateTime fromDt;
    DateTime untillDt;
    switch (crossing_type)
    {
      case "left":
        fromDt = assertedChart.FromDate.AddDays(-10);
        untillDt = assertedChart.FromDate.AddDays(1);
        break;
      case "right":
        fromDt = assertedChart.UntillDate.AddDays(-1);
        untillDt = assertedChart.UntillDate.AddDays(10);
        break;
      case "inside":
        fromDt = assertedChart.FromDate.AddDays(1);
        untillDt = assertedChart.UntillDate.AddDays(-1);
        break;
      case "same":
        fromDt = assertedChart.FromDate;
        untillDt = assertedChart.UntillDate;
        break;
      case "over":
        fromDt = assertedChart.FromDate.AddDays(-1);
        untillDt = assertedChart.UntillDate.AddDays(1);
        break;
      default:
        throw new NotImplementedException();
    }

    var newChart = mockChartFactory.CreateChart(fromDt, untillDt);
    var newCandles = mockCandleFactory.CreateCandles(newChart.FromDate, newChart.UntillDate);
    //newChart.SetServiceProvider(sp);
    newChart.AddCandles(newCandles);

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = assertedChart.Extend(newChart);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));

    Expect("Result Status is conflict", () => Assert.Equal(ResultStatus.Conflict, assertedResult.Status));

    ExpectGroup("Validation error is correct", () =>
    {
      Expect("Has one error", () => Assert.Single(assertedResult.Errors), out var assertedError);
    });

    ExpectGroup("Candles is not damaged", () =>
    {
      Expect("Count is correct", () => Assert.Equal(baseCandles.Count(), assertedChart.Candles.Count()));
      Expect("Candles contain base candles", () => Assert.True(baseCandles.All(c => assertedChart.Candles.Contains(c))));
    });

    #endregion
  }

  [Theory]
  [InlineData("left")]
  [InlineData("right")]
  public void WHEN_not_connected_THEN_validation_error(string crossing_type)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    DateTime fromDt;
    DateTime untillDt;
    switch (crossing_type)
    {
      case "left":
        fromDt = assertedChart.FromDate.AddDays(-10);
        untillDt = assertedChart.FromDate.AddDays(-1);
        break;
      case "right":
        fromDt = assertedChart.UntillDate.AddDays(1);
        untillDt = assertedChart.UntillDate.AddDays(10);
        break;
      default:
        throw new NotImplementedException();
    }
    var newChart = mockChartFactory.CreateChart(fromDt, untillDt);
    var newCandles = mockCandleFactory.CreateCandles(newChart.FromDate, newChart.UntillDate);
    //newChart.SetServiceProvider(sp);
    newChart.AddCandles(newCandles);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedResult = assertedChart.Extend(newChart);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));

    Expect("Result Status is invalid", () => Assert.Equal(ResultStatus.Error, assertedResult.Status));

    ExpectGroup("Validation error is correct", () =>
    {
      Expect("Has one error", () => Assert.Single(assertedResult.Errors), out var validationError);
    });

    ExpectGroup("Candles is not damaged", () =>
    {
      Expect("Count is correct", () => Assert.Equal(baseCandles.Count(), assertedChart.Candles.Count()));
      Expect("Candles contain base candles", () => Assert.True(baseCandles.All(c => assertedChart.Candles.Contains(c))));
    });

    #endregion
  }
}