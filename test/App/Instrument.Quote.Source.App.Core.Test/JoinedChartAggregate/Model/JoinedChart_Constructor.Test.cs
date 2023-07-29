using System.ComponentModel.DataAnnotations;
using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.JoinedChartAggregate.Model;

public class JoinedChart_Constructor_Test : ExpectationsTestBase
{
  MockChartFactory mockChartFactory = new MockChartFactory();
  Chart usedChart;
  TimeFrame usedTimeFrame;
  public JoinedChart_Constructor_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {
    usedChart = mockChartFactory.CreateChart();
    usedTimeFrame = TimeFrame.Enum.D1.ToEntity();
  }

  [Fact]
  public void WHEN_create_new_THEN_correct()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var usedFromDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var usedTillDt = new DateTime(2021, 1, 1).ToUniversalTime();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var assertedChart = new JoinedChart(usedFromDt, usedTillDt, usedChart, usedTimeFrame);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("From Date is correct", () => Assert.Equal(usedFromDt, assertedChart.FromDate));
    Expect("Untill Date is correct", () => Assert.Equal(usedTillDt, assertedChart.UntillDate));
    Expect("Chart is correct", () => Assert.Equal(usedChart.Id, assertedChart.StepChartId));
    Expect("TimeFrame is correc", () => Assert.Equal(usedTimeFrame.Id, assertedChart.TargetTimeFrameId));

    #endregion
  }

  public static IEnumerable<object[]> IncorrectDates
  {
    get
    {

      yield return new object[] { "both NOT UTC", new DateTime(2020, 1, 1), new DateTime(2020, 5, 1) };
      yield return new object[] { "Until NOT UTC", new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 5, 1) };
      yield return new object[] { "From NOT UTC", new DateTime(2020, 1, 1), new DateTime(2020, 5, 1).ToUniversalTime() };
      yield return new object[] { "From > Untill", new DateTime(2021, 1, 1).ToUniversalTime(), new DateTime(2020, 5, 1).ToUniversalTime() };
      yield return new object[] { "From = Untill", new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 1, 1).ToUniversalTime() };
    }
  }

  [Theory]
  [MemberData(nameof(IncorrectDates))]
  public void WHEN_from_or_untill_dates_incorrect_THEN_validation_error(string Name, DateTime expectedFromDt, DateTime expectedUntillDt)
  {
    Logger.LogInformation($"Check: {Name}");

    #region Array
    Logger.LogDebug("Test ARRAY");


    #endregion


    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<ValidationException>(() =>
        new JoinedChart(expectedFromDt, expectedUntillDt, usedChart, usedTimeFrame)),
      out var assertedException
    );

    Logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }

  [Fact]
  public void WHEN_chart_is_null_THEN_validation_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var expectedFromDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntillDt = new DateTime(2020, 3, 1).ToUniversalTime();
    Chart wrongChart = null;

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<NullReferenceException>(() =>
        new JoinedChart(expectedFromDt, expectedUntillDt, wrongChart, usedTimeFrame)),
      out var assertedException
    );

    Logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }

  [Fact]
  public void WHEN_timeframe_is_null_THEN_validation_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    var expectedFromDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntillDt = new DateTime(2020, 3, 1).ToUniversalTime();
    TimeFrame wrongTf = null;
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<NullReferenceException>(() =>
        new JoinedChart(expectedFromDt, expectedUntillDt, usedChart, wrongTf)),
      out var assertedException
    );

    Logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }
}