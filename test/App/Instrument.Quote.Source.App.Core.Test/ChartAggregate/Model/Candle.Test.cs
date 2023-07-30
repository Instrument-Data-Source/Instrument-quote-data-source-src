using System.ComponentModel.DataAnnotations;
using System.Net;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Model;

public class Candle_Constructor_Test : BaseTest<Candle_Constructor_Test>
{
  private Chart usedChart = new MockChartFactory().CreateChart(initId: true);
  public Candle_Constructor_Test(ITestOutputHelper output) : base(output)
  {

  }
  public static IEnumerable<object[]> CorrectData
  {
    get
    {
      foreach (var volume in new[] { 0, 10 })
      {
        yield return new object[] { 5, 5, 5, 5, volume };
        yield return new object[] { 5, 7, 2, 6, volume };
        yield return new object[] { 5, 7, 1, 4, volume };
      }
    }
  }

  [Theory]
  [MemberData(nameof(CorrectData))]
  public void WHEN_create_new_candle_with_correct_data_THEN_candle_created(int expectedOpen, int expectedHigh, int expectedLow, int expectedClose, int expectedVolume)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedDt = DateTime.UtcNow;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedCandle = new Candle(expectedDt, expectedOpen, expectedHigh, expectedLow, expectedClose, expectedVolume, usedChart);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Date is correct", () => Assert.Equal(expectedDt, assertedCandle.DateTime));
    Expect("Open is correct", () => Assert.Equal(expectedOpen, assertedCandle.Open));
    Expect("High is correct", () => Assert.Equal(expectedHigh, assertedCandle.High));
    Expect("Low is correct", () => Assert.Equal(expectedLow, assertedCandle.Low));
    Expect("Close is correct", () => Assert.Equal(expectedClose, assertedCandle.Close));
    Expect("Volume is correct", () => Assert.Equal(expectedVolume, assertedCandle.Volume));

    #endregion
  }

  [Fact]
  public void WHEN_use_non_utc_date_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var wrongDt = DateTime.Now;
    int expectedOpen = 100;
    int expectedHigh = 200;
    int expectedLow = 50;
    int expectedClose = 150;
    int expectedVolume = 300;
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<ValidationException>(() =>
        new Candle(wrongDt, expectedOpen, expectedHigh, expectedLow, expectedClose, expectedVolume, usedChart)
      ),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);
    #endregion
  }

  public static IEnumerable<object[]> WrongData
  {
    get
    {
      foreach (var volume in new[] { -1 })
      {
        yield return new object[] { 5, 4, 2, 5, volume };
        yield return new object[] { 5, 7, 6, 5, volume };
        yield return new object[] { 5, 7, 0, 5, volume };
        yield return new object[] { 5, 7, -2, 5, volume };
        yield return new object[] { -5, -5, -5, -5, volume };
      }
    }
  }
  [Theory]
  [MemberData(nameof(WrongData))]
  public void WHEN_wrong_candle_values_THEN_error(int expectedOpen, int expectedHigh, int expectedLow, int expectedClose, int expectedVolume)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expectedDt = DateTime.UtcNow;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<ValidationException>(() =>
         new Candle(expectedDt, expectedOpen, expectedHigh, expectedLow, expectedClose, expectedVolume, usedChart)
      ),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);
    #endregion
  }

  [Fact]
  public void WHEN_give_null_chart_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedDt = DateTime.UtcNow;
    var expectedOpen = 50;
    var expectedHigh = 100;
    var expectedLow = 25;
    var expectedClose = 50;
    var expectedVolume = 0;
    Chart wrongChart = null;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");


    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<NullReferenceException>(() =>
         new Candle(expectedDt, expectedOpen, expectedHigh, expectedLow, expectedClose, expectedVolume, wrongChart)
      ),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }

}