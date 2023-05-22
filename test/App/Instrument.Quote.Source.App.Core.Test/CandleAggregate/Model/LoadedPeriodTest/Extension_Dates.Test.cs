namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Model.LoadedPeriodTest;

using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.TestData;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;
public class Extend_Dates_Test
{

  LoadedPeriod basePeriod;
  IEnumerable<Candle> baseCandles;
  int baseInstument = 0;
  TimeFrame.Enum baseTimeFrame = TimeFrame.Enum.D1;
  private readonly ITestOutputHelper output;

  public Extend_Dates_Test(ITestOutputHelper output)
  {
    baseCandles = CandleFactory.RandomCandles(5, new DateTime(2020, 1, 13).ToUniversalTime());
    basePeriod = LoadedPeriod.BuildNewPeriod(
              baseInstument,
              baseTimeFrame,
              new DateTime(2020, 1, 10).ToUniversalTime(),
              new DateTime(2020, 1, 20).ToUniversalTime(), baseCandles);
    this.output = output;
  }

  [Fact]
  public void WHEN_extend_THEN_correct_return_same_instance()
  {
    // Array

    var newCandles = CandleFactory.RandomCandles(5, new DateTime(2020, 1, 2).ToUniversalTime());
    var newPeriod = LoadedPeriod.BuildNewPeriod(
      baseInstument,
      baseTimeFrame,
      new DateTime(2020, 1, 1).ToUniversalTime(),
      new DateTime(2020, 1, 10).ToUniversalTime(),
      newCandles);

    // Act
    var asserted_ent = basePeriod.Extend(newPeriod);


    // Assert
    Assert.True(basePeriod == asserted_ent, "Extend must return same instance");
  }

  #region Check DateTime work
  public static IEnumerable<object[]> Connected_DateTimeCases
  {
    get
    {
      yield return new object[] {
        new DateTime(2020,1,1), new DateTime(2020,1,10),
        new DateTime(2020,1,1), new DateTime(2020,1,20), };
      yield return new object[] {
        new DateTime(2020,1,20), new DateTime(2020,1,21),
        new DateTime(2020,1,10), new DateTime(2020,1,21), };
    }
  }
  [Theory]
  [MemberData(nameof(Connected_DateTimeCases))]
  public void WHEN_extend_period_is_connected_THEN_from_dt_and_untill_dt_change_correctly(
      DateTime newFromDt, DateTime newUntillDt,
      DateTime expectedFromDt, DateTime expectedUntillDt)
  {
    // Array
    newFromDt = newFromDt.ToUniversalTime();
    newUntillDt = newUntillDt.ToUniversalTime();
    expectedFromDt = expectedFromDt.ToUniversalTime();
    expectedUntillDt = expectedUntillDt.ToUniversalTime();

    var newPeriod = new LoadedPeriod(baseInstument, baseTimeFrame, newFromDt, newUntillDt);
    // Act
    var assertedPeriod = basePeriod.Extend(newPeriod);
    // Assert
    Assert.Equal(expectedFromDt, assertedPeriod.FromDate);
    Assert.Equal(expectedUntillDt, assertedPeriod.UntillDate);
  }

  public static IEnumerable<object[]> Cross_DateTimeCases
  {
    get
    {
      yield return new object[] {
        new DateTime(2020,1,1), new DateTime(2020,1,15),
        new DateTime(2020,1,1), new DateTime(2020,1,20), };
      yield return new object[] {
        new DateTime(2020,1,1), new DateTime(2020,1,20),
        new DateTime(2020,1,1), new DateTime(2020,1,20), };
      yield return new object[] {
        new DateTime(2020,1,1), new DateTime(2020,1,21),
        new DateTime(2020,1,1), new DateTime(2020,1,21), };
      yield return new object[] {
        new DateTime(2020,1,10), new DateTime(2020,1,15),
        new DateTime(2020,1,10), new DateTime(2020,1,20), };
      yield return new object[] {
        new DateTime(2020,1,10), new DateTime(2020,1,20),
        new DateTime(2020,1,10), new DateTime(2020,1,20), };
      yield return new object[] {
        new DateTime(2020,1,10), new DateTime(2020,1,21),
        new DateTime(2020,1,10), new DateTime(2020,1,21), };
    }
  }
  [Theory]
  [MemberData(nameof(Cross_DateTimeCases))]
  public void WHEN_extended_period_is_cross_THEN_from_dt_and_untill_dt_change_correctly(
      DateTime newFromDt, DateTime newUntillDt,
      DateTime expectedFromDt, DateTime expectedUntillDt)
  {
    // Array
    newFromDt = newFromDt.ToUniversalTime();
    newUntillDt = newUntillDt.ToUniversalTime();
    expectedFromDt = expectedFromDt.ToUniversalTime();
    expectedUntillDt = expectedUntillDt.ToUniversalTime();

    var newPeriod = new LoadedPeriod(baseInstument, baseTimeFrame, newFromDt, newUntillDt);
    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => basePeriod.Extend(newPeriod)); ;
  }

  public static IEnumerable<object[]> NoCross_NoConnected_DateTimeCases
  {
    get
    {
      yield return new object[] {
        new DateTime(2020,1,1), new DateTime(2020,1,9)};
      yield return new object[] {
        new DateTime(2020,1,21), new DateTime(2020,1,22) };
    }
  }
  [Theory]
  [MemberData(nameof(NoCross_NoConnected_DateTimeCases))]
  public void WHEN_extend_unsuccess_THEN_exception(
      DateTime newFromDt, DateTime newUntillDt)
  {
    // Array
    newFromDt = newFromDt.ToUniversalTime();
    newUntillDt = newUntillDt.ToUniversalTime();

    var newPeriod = new LoadedPeriod(baseInstument, baseTimeFrame, newFromDt, newUntillDt);
    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => basePeriod.Extend(newPeriod));
  }

  public static IEnumerable<object[]> Wrong_Instrument_Or_TimeFrame
  {
    get
    {
      yield return new object[] { 99, TimeFrame.Enum.M };
      yield return new object[] { 99, null };
      yield return new object[] { null, TimeFrame.Enum.M };
    }
  }

  [Theory]
  [MemberData(nameof(Wrong_Instrument_Or_TimeFrame))]
  public void WHEN_give_another_instument_or_timeframe_THEN_exception(int? instrumentId, TimeFrame.Enum? timeFrame)
  {
    // Array
    var usingInstrumentId = instrumentId ?? basePeriod.InstrumentId;
    var usingTimeFrameId = timeFrame ?? (TimeFrame.Enum)basePeriod.TimeFrameId;
    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() =>
          basePeriod.Extend(new LoadedPeriod(usingInstrumentId, usingTimeFrameId,
          new DateTime(2020, 1, 1).ToUniversalTime(),
          new DateTime(2020, 1, 10).ToUniversalTime())));
  }
  #endregion

}