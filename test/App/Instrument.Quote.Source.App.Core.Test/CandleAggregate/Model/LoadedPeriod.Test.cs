namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Model;

using System.ComponentModel.DataAnnotations;
using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
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
    var expected_candles = mockCandleFactory.CreateCandles(expected_from, expected_untill);
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

      yield return new object[] { "both NOT UTC", new DateTime(2020, 1, 1), new DateTime(2020, 5, 1) };
      yield return new object[] { "Until NOT UTC", new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 5, 1) };
      yield return new object[] { "From NOT UTC", new DateTime(2020, 1, 1), new DateTime(2020, 5, 1).ToUniversalTime() };
      yield return new object[] { "From > Untill", new DateTime(2021, 1, 1).ToUniversalTime(), new DateTime(2020, 5, 1).ToUniversalTime() };
      yield return new object[] { "From = Untill", new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 1, 1).ToUniversalTime() };
    }
  }

  [Theory]
  [MemberData(nameof(IncorrectDates))]
  public void WHEN_from_or_untill_dates_incorrect_THEN_validation_error(string Name, DateTime fromDt, DateTime untillDt)
  {
    logger.LogInformation($"Check: {Name}");

    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expected_candles = mockCandleFactory.CreateCandles(2, fromDt);

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
    var expected_candles = mockCandleFactory.CreateCandles(fromDate, untillDate);

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
    var expected_candles = mockCandleFactory.CreateCandles(expected_from, expected_untill);
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

  [Fact]
  public void WHEN_candles_datetime_doesnt_fit_to_timeframe_THEN_throw_validation_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expected_from = new DateTime(2020, 1, 1).ToUniversalTime();
    var expected_untill = new DateTime(2020, 1, 3).ToUniversalTime();
    var expected_candles = mockCandleFactory.CreateCandles(expected_from, expected_untill, hourStep: true);

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
}


public class LoadedPeriad_Extend_Test : BaseTest<LoadedPeriad_Extend_Test>
{
  ent.Instrument mockInstrument = MockInstrument.Create();
  TimeFrame mockTimeFrame = TimeFrame.Enum.D1.ToEntity();

  MockPeriodFactory periodFactory;

  LoadedPeriod usedPeriod;

  public LoadedPeriad_Extend_Test(ITestOutputHelper output) : base(output)
  {
    periodFactory = new MockPeriodFactory(mockInstrument, mockTimeFrame);

    usedPeriod = periodFactory.CreatePeriod(initId: true);
  }

  [Fact]
  public void WHEN_new_period_has_id_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var newPeriod = periodFactory.CreatePeriod(usedPeriod.FromDate.AddDays(-10), usedPeriod.FromDate, true);

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    /*
    Expect("Extend method throw Validation exception", () =>
      Assert.Throws<FluentValidation.ValidationException>(() => usedPeriod.Extend(newPeriod)),
      out var assertedException);

    Expect("Exception has one error", () =>
      Assert.Single(assertedException.Errors),
      out var assertedFailure);
    this.logger.LogInformation($"ValidationFailure: {assertedFailure.ErrorMessage}");


    Expect("Property name is Id", () =>
      Assert.Equal(nameof(LoadedPeriod.Id), assertedFailure.PropertyName));
    */

    Expect("Extend method throw Validation exception", () =>
      Assert.Throws<ValidationException>(() => usedPeriod.Extend(newPeriod)),
      out var assertedException);

    Expect("Exception has one error", () =>
      Assert.Single((assertedException.InnerException as AggregateException).InnerExceptions),
      out Exception assertedInnerException);
    this.logger.LogInformation($"ValidationException: {assertedInnerException.Message}");

    var assertedValidationException = (ValidationException)assertedInnerException;

    Expect("Single member name", () =>
      Assert.Single(assertedValidationException.ValidationResult.MemberNames),
      out var assertedMemberName);
    Expect("Member name is Id", () =>
      Assert.Equal(nameof(LoadedPeriod.Id), assertedMemberName));

    #endregion
  }

  [Theory]
  [InlineData("left")]
  [InlineData("right")]
  [InlineData("inside")]
  [InlineData("same")]
  [InlineData("over")]
  public void WHEN_dates_are_crossing_THEN_validation_error(string crossing_type)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    DateTime fromDt;
    DateTime untillDt;
    switch (crossing_type)
    {
      case "left":
        fromDt = usedPeriod.FromDate.AddDays(-10);
        untillDt = usedPeriod.FromDate.AddDays(1);
        break;
      case "right":
        fromDt = usedPeriod.UntillDate.AddDays(-1);
        untillDt = usedPeriod.UntillDate.AddDays(10);
        break;
      case "inside":
        fromDt = usedPeriod.FromDate.AddDays(1);
        untillDt = usedPeriod.UntillDate.AddDays(-1);
        break;
      case "same":
        fromDt = usedPeriod.FromDate;
        untillDt = usedPeriod.UntillDate;
        break;
      case "over":
        fromDt = usedPeriod.FromDate.AddDays(-1);
        untillDt = usedPeriod.UntillDate.AddDays(1);
        break;
      default:
        throw new NotImplementedException();
    }
    var newPeriod = periodFactory.CreatePeriod(fromDt, untillDt, false);

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    /*
    Expect("Extend method throw Validation exception", () =>
      Assert.Throws<FluentValidation.ValidationException>(() => usedPeriod.Extend(newPeriod)),
      out var assertedException);
    logger.LogInformation($"ValidationException message: {assertedException.Message}");

    Expect("Exception has 2 errors", () =>
      Assert.Equal(2, assertedException.Errors.Count()));

    Expect($"One error is about {nameof(LoadedPeriod.FromDate)}", () =>
      Assert.Contains(nameof(LoadedPeriod.FromDate), assertedException.Errors.Select(e => e.PropertyName)));
    Expect($"Second error is about {nameof(LoadedPeriod.UntillDate)}", () =>
      Assert.Contains(nameof(LoadedPeriod.UntillDate), assertedException.Errors.Select(e => e.PropertyName)));
    */

    Expect("Extend method throw Validation exception", () =>
    Assert.Throws<ValidationException>(() => usedPeriod.Extend(newPeriod)),
    out var assertedException);

    Expect("Exception has one error", () =>
      Assert.Single((assertedException.InnerException as AggregateException).InnerExceptions),
      out Exception assertedInnerException);
    this.logger.LogInformation($"ValidationException: {assertedInnerException.Message}");

    var assertedValidationException = (ValidationException)assertedInnerException;

    ExpectGroup("Two member name", () =>
    {
      Expect("From is one of member name", () =>
        Assert.Contains(nameof(LoadedPeriod.FromDate), assertedValidationException.ValidationResult.MemberNames));
      Expect("Untill is one of member name", () =>
        Assert.Contains(nameof(LoadedPeriod.UntillDate), assertedValidationException.ValidationResult.MemberNames));
    });

    #endregion
  }

  [Theory]
  [InlineData("left")]
  [InlineData("right")]
  public void WHEN_not_connected_THEN_validation_error(string crossing_type)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    DateTime fromDt;
    DateTime untillDt;
    switch (crossing_type)
    {
      case "left":
        fromDt = usedPeriod.FromDate.AddDays(-10);
        untillDt = usedPeriod.FromDate.AddDays(-1);
        break;
      case "right":
        fromDt = usedPeriod.UntillDate.AddDays(1);
        untillDt = usedPeriod.UntillDate.AddDays(10);
        break;
      default:
        throw new NotImplementedException();
    }
    var newPeriod = periodFactory.CreatePeriod(fromDt, untillDt, false);

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    /*
    Expect("Extend method throw Validation exception", () =>
      Assert.Throws<FluentValidation.ValidationException>(() => usedPeriod.Extend(newPeriod)),
      out var assertedException);
    logger.LogInformation($"ValidationException message: {assertedException.Message}");

    Expect("Exception has 2 errors", () =>
      Assert.Equal(2, assertedException.Errors.Count()));

    Expect($"One error is about {nameof(LoadedPeriod.FromDate)}", () =>
      Assert.Contains(nameof(LoadedPeriod.FromDate), assertedException.Errors.Select(e => e.PropertyName)));
    Expect($"Second error is about {nameof(LoadedPeriod.UntillDate)}", () =>
      Assert.Contains(nameof(LoadedPeriod.UntillDate), assertedException.Errors.Select(e => e.PropertyName)));
    */


    Expect("Extend method throw Validation exception", () =>
    Assert.Throws<ValidationException>(() => usedPeriod.Extend(newPeriod)),
    out var assertedException);

    Expect("Exception has one error", () =>
      Assert.Single((assertedException.InnerException as AggregateException).InnerExceptions),
      out Exception assertedInnerException);
    this.logger.LogInformation($"ValidationException: {assertedInnerException.Message}");

    var assertedValidationException = (ValidationException)assertedInnerException;

    ExpectGroup("Two member name", () =>
    {
      Expect("From is one of member name", () =>
        Assert.Contains(nameof(LoadedPeriod.FromDate), assertedValidationException.ValidationResult.MemberNames));
      Expect("Untill is one of member name", () =>
        Assert.Contains(nameof(LoadedPeriod.UntillDate), assertedValidationException.ValidationResult.MemberNames));
    });

    #endregion
  }


  [Theory]
  [InlineData("left")]
  [InlineData("right")]
  public void WHEN_period_connected_THEN_update_period(string crossing_type)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    DateTime fromDt;
    DateTime untillDt;
    DateTime expectedFromDt;
    DateTime expectedUntillDt;
    switch (crossing_type)
    {
      case "left":
        fromDt = usedPeriod.FromDate.AddDays(-10);
        untillDt = usedPeriod.FromDate;
        expectedFromDt = fromDt;
        expectedUntillDt = usedPeriod.UntillDate;
        break;
      case "right":
        fromDt = usedPeriod.UntillDate;
        untillDt = usedPeriod.UntillDate.AddDays(10);
        expectedFromDt = usedPeriod.FromDate;
        expectedUntillDt = untillDt;
        break;
      default:
        throw new NotImplementedException();
    }
    var newPeriod = periodFactory.CreatePeriod(fromDt, untillDt, false);


    var expectedCandlesCount = usedPeriod.Candles.Count() + newPeriod.Candles.Count();
    var expectedCandleDtList = new List<DateTime>();
    expectedCandleDtList.AddRange(usedPeriod.Candles.Select(e => e.DateTime));
    expectedCandleDtList.AddRange(newPeriod.Candles.Select(e => e.DateTime));
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedPeriod = usedPeriod.Extend(newPeriod);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Return same instance", () => Assert.True(usedPeriod == assertedPeriod));

    Expect("Same ID of returned period", () => Assert.Equal(usedPeriod.Id, assertedPeriod.Id));

    ExpectGroup("New period has extended dates", () =>
    {
      Expect("From date is extend correctly", () => Assert.Equal(expectedFromDt, assertedPeriod.FromDate));
      Expect("Untill date is extend correctly", () => Assert.Equal(expectedUntillDt, assertedPeriod.UntillDate));
    });

    ExpectGroup("Candles exptend correctly", () =>
    {
      Expect("Count is summed", () => Assert.Equal(expectedCandlesCount, assertedPeriod.Candles.Count()));
      Expect("All Candles in returned period", () => Assert.True(assertedPeriod.Candles.All(c => expectedCandleDtList.Contains(c.DateTime))));
    });

    #endregion
  }
}