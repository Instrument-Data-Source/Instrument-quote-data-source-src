namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Model;

using System.ComponentModel.DataAnnotations;
using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
public class LoadedPeriod_Construction_Test : BaseTest<LoadedPeriod_Construction_Test>
{
  ent.Instrument mockInstrument1 = MockInstrument.Create();
  TimeFrame usedTimeframe = TimeFrame.Enum.D1.ToEntity();
  MockCandleFactory mockCandleFactory;
  public LoadedPeriod_Construction_Test(ITestOutputHelper output) : base(output)
  {
    mockCandleFactory = new MockCandleFactory(mockInstrument1, usedTimeframe);
  }

  [Fact]
  public void WHEN_give_correct_data_THEN_create_correct_period()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expected_from = new DateTime(2020, 1, 1).ToUniversalTime();
    var expected_untill = new DateTime(2020, 1, 11).ToUniversalTime();
    var expected_candles = mockCandleFactory.CreateRandomCandles(expected_from, expected_untill);
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedPeriod = new LoadedPeriod(expected_from, expected_untill, mockInstrument1, usedTimeframe, expected_candles);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("FromDate is correct", () => Assert.Equal(expected_from, assertedPeriod.FromDate));
    Expect("Untill is correct", () => Assert.Equal(expected_untill, assertedPeriod.UntillDate));
    Expect("InstrumentId is correct", () => Assert.Equal(mockInstrument1.Id, assertedPeriod.InstrumentId));
    Expect("Instrument is correct", () => Assert.Equal(mockInstrument1, assertedPeriod.Instrument));
    Expect("TimeFrameId is correct", () => Assert.Equal(usedTimeframe.Id, assertedPeriod.TimeFrameId));
    Expect("TimeFrame is correct", () => Assert.Equal(usedTimeframe, assertedPeriod.TimeFrame));
    ExpectGroup("Candles is correct", () =>
    {
      Expect("Candles array has correct count of entities", () => Assert.Equal(expected_candles.Count(), assertedPeriod.Candles.Count()));
      Expect("All candles in list", () =>
      {
        foreach (var expected_candle in expected_candles)
        {
          Assert.Contains(expected_candle, assertedPeriod.Candles);
        }
      });
    });
    #endregion
  }

  public static IEnumerable<object[]> IncorrectDates
  {
    get
    {

      yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2020, 5, 1) };
      yield return new object[] { new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 5, 1) };
      yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2020, 5, 1).ToUniversalTime() };
      yield return new object[] { new DateTime(2021, 1, 1).ToUniversalTime(), new DateTime(2020, 5, 1).ToUniversalTime() };
      yield return new object[] { new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 1, 1).ToUniversalTime() };
    }
  }

  [Theory]
  [MemberData(nameof(IncorrectDates))]
  public void WHEN_dates_incorrect_THEN_error(DateTime fromDt, DateTime untillDt)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expected_candles = mockCandleFactory.CreateRandomCandles(2, fromDt);

    #endregion

    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion

    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Constructor throw ValidationException", () =>
      Assert.Throws<ValidationException>(() => new LoadedPeriod(fromDt, untillDt, mockInstrument1, usedTimeframe, expected_candles)),
      out var assertedException);

    logger.LogInformation("Asserted exception message: " + assertedException.Message);
    #endregion
  }

  [Fact]
  public void WHEN_no_candles_in_list_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expected_from = new DateTime(2020, 1, 1).ToUniversalTime();
    var expected_untill = new DateTime(2020, 1, 11).ToUniversalTime();
    var expected_candles = new List<Candle>();
    #endregion



    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Constructor throw ValidationException", () =>
      Assert.Throws<ValidationException>(() => new LoadedPeriod(expected_from, expected_untill, mockInstrument1, usedTimeframe, expected_candles)),
      out var assertedException);

    Expect("Exception has one error", () =>
      Assert.Single((assertedException.InnerException as AggregateException).InnerExceptions),
      out Exception assertedInnerException);
    this.logger.LogInformation($"ValidationException: {assertedInnerException.Message}");

    var assertedValidationException = (ValidationException)assertedInnerException;

    Expect("Single member name", () =>
      Assert.Single(assertedValidationException.ValidationResult.MemberNames),
      out var assertedMemberName);
    Expect("Member name is Candles", () =>
      Assert.Equal(nameof(LoadedPeriod.Candles), assertedMemberName));

    logger.LogInformation("Asserted exception message: " + assertedException.Message);
    #endregion
  }
  public static IEnumerable<object[]> CandlesOutOfRange
  {
    get
    {
      yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2020, 2, 1) };
      yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2020, 4, 1) };
      yield return new object[] { new DateTime(2020, 1, 1), new DateTime(2020, 6, 1) };
      yield return new object[] { new DateTime(2020, 4, 1), new DateTime(2020, 6, 1) };
      yield return new object[] { new DateTime(2020, 6, 1), new DateTime(2020, 8, 1) };
      yield return new object[] { new DateTime(2020, 3, 1), new DateTime(2020, 5, 2) };
    }
  }
  [Theory]
  [MemberData(nameof(CandlesOutOfRange))]
  public void WHEN_candles_is_out_of_period_range_THEN_error(DateTime fromDate, DateTime untillDate)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expected_from = new DateTime(2020, 3, 1).ToUniversalTime();
    var expected_untill = new DateTime(2020, 5, 1).ToUniversalTime();
    var expected_candles = mockCandleFactory.CreateRandomCandles(fromDate, untillDate);

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Constructor throw ValidationException", () =>
      Assert.Throws<ValidationException>(() => new LoadedPeriod(expected_from, expected_untill, mockInstrument1, usedTimeframe, expected_candles)),
      out var assertedException);

    Expect("Exception has one error", () =>
      Assert.Single((assertedException.InnerException as AggregateException).InnerExceptions),
      out Exception assertedInnerException);
    this.logger.LogInformation($"ValidationException: {assertedInnerException.Message}");

    var assertedValidationException = (ValidationException)assertedInnerException;

    Expect("Single member name", () =>
      Assert.Single(assertedValidationException.ValidationResult.MemberNames),
      out var assertedMemberName);
    Expect("Member name is Candles", () =>
      Assert.Equal(nameof(LoadedPeriod.Candles), assertedMemberName));

    logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }

  [Fact]
  public void WHEN_candles_has_duplicate_datetime_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expected_from = new DateTime(2020, 1, 1).ToUniversalTime();
    var expected_untill = new DateTime(2020, 1, 11).ToUniversalTime();
    var expected_candles = mockCandleFactory.CreateRandomCandles(expected_from, expected_untill);
    expected_candles = expected_candles.Append(expected_candles.ElementAt(1));
    #endregion  


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    this.logger.LogDebug("Test ASSERT");

    Expect("Constructor throw ValidationException", () =>
      Assert.Throws<ValidationException>(() => new LoadedPeriod(expected_from, expected_untill, mockInstrument1, usedTimeframe, expected_candles)),
      out var assertedException);

    Expect("Exception has one error", () =>
      Assert.Single((assertedException.InnerException as AggregateException).InnerExceptions),
      out Exception assertedInnerException);
    this.logger.LogInformation($"ValidationException: {assertedInnerException.Message}");

    var assertedValidationException = (ValidationException)assertedInnerException;

    Expect("Single member name", () =>
      Assert.Single(assertedValidationException.ValidationResult.MemberNames),
      out var assertedMemberName);
    Expect("Member name is Candles", () =>
      Assert.Equal(nameof(LoadedPeriod.Candles), assertedMemberName));

    logger.LogInformation("Asserted exception message: " + assertedException.Message);

    #endregion
  }
}