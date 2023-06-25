namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Tool.PeriodJoinerTest;

using Xunit.Abstractions;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
/*
public class Extend_Test
{

  LoadedPeriod basePeriod;
  IEnumerable<Candle> baseCandles;
  int baseInstument = 0;
  TimeFrame.Enum baseTimeFrame = TimeFrame.Enum.D1;
  private readonly ITestOutputHelper output;

  public Extend_Test(ITestOutputHelper output)
  {
    baseCandles = CandleFactory.RandomCandles(5, new DateTime(2020, 1, 13).ToUniversalTime());
    basePeriod = new LoadedPeriod(baseInstument, baseTimeFrame, new DateTime(2020, 1, 10).ToUniversalTime(), new DateTime(2020, 1, 20).ToUniversalTime())
                      .AddCandles(baseCandles);
    this.output = output;
  }

  [Fact]
  public void WHEN_extend_THEN_correct_return_period()
  {
    // Array
    var newPeriod = new LoadedPeriod(baseInstument, baseTimeFrame, new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 1, 10).ToUniversalTime());
    var newCandles = CandleFactory.RandomCandles(5, new DateTime(2020, 1, 2).ToUniversalTime());
    newPeriod.AddCandles(newCandles);

    // Act
    var assertedPeriod = basePeriod.Extend(newPeriod);

    // Assert
    Assert.True(basePeriod == assertedPeriod, "Return instance must be same as given");

    output.WriteLine("Candles in result list must be as sum, if all unique");
    Assert.Equal(baseCandles.Count() + newCandles.Count(), assertedPeriod.Candles.Count());
    foreach (var candle in baseCandles)
    {
      Assert.Contains(candle, assertedPeriod.Candles);
    }
    foreach (var candle in newCandles)
    {
      Assert.Contains(candle, assertedPeriod.Candles);
    }
  }
  public static IEnumerable<object[]> Success_DateTimeCases
  {
    get
    {
      yield return new object[] {
        new DateTime(2020,1,1), new DateTime(2020,1,10),
        new DateTime(2020,1,1), new DateTime(2020,1,20), };
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
      yield return new object[] {
        new DateTime(2020,1,20), new DateTime(2020,1,21),
        new DateTime(2020,1,10), new DateTime(2020,1,21), };
    }
  }
  [Theory]
  [MemberData(nameof(Success_DateTimeCases))]
  public void WHEN_extend_THEN_from_dt_and_untill_dt_change_correctly(
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

  public static IEnumerable<object[]> UnSuccess_DateTimeCases
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
  [MemberData(nameof(UnSuccess_DateTimeCases))]
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

  [Fact]
  public void WHEN_give_another_instument_or_timeframe_THEN_exceptio()
  {
    // Array

    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => new LoadedPeriod(99, TimeFrame.Enum.M, basePeriod.FromDate, basePeriod.UntillDate));
    Assert.Throws<FluentValidation.ValidationException>(() => new LoadedPeriod(99, basePeriod.TimeFrameId, basePeriod.FromDate, basePeriod.UntillDate));
    Assert.Throws<FluentValidation.ValidationException>(() => new LoadedPeriod(basePeriod.InstrumentId, TimeFrame.Enum.M, basePeriod.FromDate, basePeriod.UntillDate));
  }
}*/