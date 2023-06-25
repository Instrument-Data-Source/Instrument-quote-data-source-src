namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Model;

using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public class Candle_Construct_Test : BaseTest<Candle_Construct_Test>
{
  ent.Instrument mockInstrument1 = MockInstrument.Create();
  TimeFrame usedTimeframe = TimeFrame.Enum.D1.ToEntity();
  public Candle_Construct_Test(ITestOutputHelper output) : base(output)
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
  public void WHEN_create_new_candle_with_correct_data_THEN_candle_created(int expected_open, int expected_high, int expected_low, int expected_close, int expected_volume)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expected_dt = DateTime.UtcNow;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    var assertedCandle = new Candle(expected_dt,
                                    expected_open,
                                    expected_high,
                                    expected_low,
                                    expected_close,
                                    expected_volume,
                                    mockInstrument1, usedTimeframe);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("DateTime is correct", () => Assert.Equal(expected_dt, assertedCandle.DateTime));
    Expect("Open is correct", () => Assert.Equal(expected_open, assertedCandle.OpenStore));
    Expect("High is correct", () => Assert.Equal(expected_high, assertedCandle.HighStore));
    Expect("Low is correct", () => Assert.Equal(expected_low, assertedCandle.LowStore));
    Expect("Close is correct", () => Assert.Equal(expected_close, assertedCandle.CloseStore));
    Expect("Volume is correct", () => Assert.Equal(expected_volume, assertedCandle.VolumeStore));
    Expect("Instrument ID is correct", () => Assert.Equal(mockInstrument1.Id, assertedCandle.InstrumentId));
    Expect("Instrument is correct", () => Assert.Equal(mockInstrument1, assertedCandle.Instrument));
    Expect("TimeFrame ID is correct", () => Assert.Equal(usedTimeframe.Id, assertedCandle.TimeFrameId));
    Expect("TimeFrame is correct", () => Assert.Equal(usedTimeframe, assertedCandle.TimeFrame));

    #endregion
  }

  [Fact]
  public void WHEN_use_non_utc_date_THEN_error()
  {
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
        new Candle(DateTime.Now,
                  100,
                  200,
                  50,
                  75,
                  100,
                  mockInstrument1, usedTimeframe)
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
  public void WHEN_wrong_candle_values_THEN_error(int expected_open, int expected_high, int expected_low, int expected_close, int expected_volume)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expected_dt = DateTime.UtcNow;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<ValidationException>(() =>
         new Candle(expected_dt,
                    expected_open,
                    expected_high,
                    expected_low,
                    expected_close,
                    expected_volume,
                    mockInstrument1, usedTimeframe)
      ),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);
    #endregion
  }

  [Fact]
  public void WHEN_instrument_null_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expected_dt = DateTime.UtcNow;
    var expected_open = 50;
    var expected_high = 100;
    var expected_low = 25;
    var expected_close = 50;
    var expected_volume = 0;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<ArgumentNullException>(() =>
         new Candle(expected_dt,
                    expected_open,
                    expected_high,
                    expected_low,
                    expected_close,
                    expected_volume,
                    null, usedTimeframe)
      ),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }

  [Fact]
  public void WHEN_timeframe_null_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expected_dt = DateTime.UtcNow;
    var expected_open = 50;
    var expected_high = 100;
    var expected_low = 25;
    var expected_close = 50;
    var expected_volume = 0;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Raise expection", () =>
      Assert.Throws<ArgumentNullException>(() =>
         new Candle(expected_dt,
                    expected_open,
                    expected_high,
                    expected_low,
                    expected_close,
                    expected_volume,
                    mockInstrument1, null)
      ),
      out var assertedException
    );

    logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }
}
