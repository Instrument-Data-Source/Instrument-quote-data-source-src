using System.ComponentModel.DataAnnotations;
using System.Net;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Model;

using System.Net;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;
using static Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks.MockChartFactory;

public class Chart_Constructor_Test : BaseTest<Chart_Constructor_Test>
{
  private ent.Instrument usedInstrument = MockInstrument.Create();
  private TimeFrame usedTf = TimeFrame.Enum.D1.ToEntity();
 // private IServiceProvider sp = Substitute.For<IServiceProvider>();
  //private Chart.Factory chartFactory;
  public Chart_Constructor_Test(ITestOutputHelper output) : base(output)
  {
   // chartFactory = new Chart.Factory(sp);
  }

  [Fact]
  public void WHEN_give_correct_data_THEN_create_chart()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedFromDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntillDt = new DateTime(2020, 3, 1).ToUniversalTime();

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedChart = new Chart(expectedFromDt, expectedUntillDt, usedInstrument, usedTf);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("From date is correct", () => Assert.Equal(expectedFromDt, assertedChart.FromDate));
    Expect("Untill date is correct", () => Assert.Equal(expectedUntillDt, assertedChart.UntillDate));
    Expect("Instrument id is correct", () => Assert.Equal(usedInstrument.Id, assertedChart.InstrumentId));
    Expect("Instrument is the same", () => Assert.True(usedInstrument == assertedChart.Instrument));
    Expect("Timeframe id is correct", () => Assert.Equal(usedTf.Id, assertedChart.TimeFrameId));
    Expect("Timeframe is the same", () => Assert.True(usedTf == assertedChart.TimeFrame));

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
    logger.LogInformation($"Check: {Name}");

    #region Array
    this.logger.LogDebug("Test ARRAY");


    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<ValidationException>(() =>
        new Chart(expectedFromDt, expectedUntillDt, usedInstrument, usedTf)),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }

  [Fact]
  public void WHEN_instrumnent_is_null_THEN_validation_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedFromDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntillDt = new DateTime(2020, 3, 1).ToUniversalTime();
    ent.Instrument wrongInstrument = null;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<NullReferenceException>(() =>
        new Chart(expectedFromDt, expectedUntillDt, wrongInstrument, usedTf)),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }

  [Fact]
  public void WHEN_timeframe_is_null_THEN_validation_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedFromDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntillDt = new DateTime(2020, 3, 1).ToUniversalTime();
    TimeFrame wrongTf = null;
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<NullReferenceException>(() =>
        new Chart(expectedFromDt, expectedUntillDt, usedInstrument, wrongTf)),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }
}
