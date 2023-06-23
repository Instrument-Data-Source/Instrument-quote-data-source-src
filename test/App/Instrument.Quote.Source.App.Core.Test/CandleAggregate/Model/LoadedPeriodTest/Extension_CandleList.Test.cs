namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Model.LoadedPeriodTest;

using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.TestData;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;
public class Extend_CandleList_Test
{
  /*

  LoadedPeriod basePeriod;
  IEnumerable<Candle> baseCandles;
  ent.Instrument baseInstument = new ent.Instrument("test1", "t1", 2, 2, 1);
  TimeFrame baseTimeFrame = new TimeFrame(TimeFrame.Enum.D1);
  private readonly ITestOutputHelper output;

  public Extend_CandleList_Test(ITestOutputHelper output)
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

  [Fact]
  public void WHEN_add_candles_before_exist_data_THEN_add_correctly()
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
    var assertedPeriod = basePeriod.Extend(newPeriod);

    // Assert
    output.WriteLine("Candles in result list must contain all DateTimes, if all unique");
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

  [Fact]
  [Trait("Group", "add non cross candles")]
  public void WHEN_add_candles_after_exist_data_THEN_add_correctly()
  {
    // Array
    var newCandles = CandleFactory.RandomCandles(5, new DateTime(2020, 1, 20).ToUniversalTime());
    var newPeriod = LoadedPeriod.BuildNewPeriod(
      baseInstument,
      baseTimeFrame,
      new DateTime(2020, 1, 20).ToUniversalTime(),
      new DateTime(2020, 1, 30).ToUniversalTime(),
      newCandles);

    // Act
    var assertedPeriod = basePeriod.Extend(newPeriod);

    // Assert
    output.WriteLine("Candles in result list must contain all DateTimes, if all unique");
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

  [Fact]
  [Trait("Group", "add cross candles")]
  //public void WHEN_correct_THEN_check_cross_candles()
  public void WHEN_correct_THEN_not_allowed()
  {
    // Array
    var newCandles = CandleFactory.RandomCandles(5, new DateTime(2020, 1, 3).ToUniversalTime());
    var crossCandles = new[] { baseCandles.ElementAt(0), baseCandles.ElementAt(1) };

    var extendedCandles = new List<Candle>();
    extendedCandles.AddRange(newCandles);
    extendedCandles.AddRange(crossCandles);

    var newPeriod = LoadedPeriod.BuildNewPeriod(
      baseInstument,
      baseTimeFrame,
      new DateTime(2020, 1, 3).ToUniversalTime(),
      new DateTime(2020, 1, 18).ToUniversalTime(),
      extendedCandles);

    Assert.Throws<FluentValidation.ValidationException>(() => basePeriod.Extend(newPeriod));

    // Act
    //var assertedPeriod = basePeriod.Extend(newPeriod);

    // Assert
    //output.WriteLine("Candles in result list must contain all DateTimes, if all unique");
    //Assert.Equal(baseCandles.Count() + newCandles.Count(), assertedPeriod.Candles.Count());
    //foreach (var candle in baseCandles)
    //{
    //  Assert.Contains(candle, assertedPeriod.Candles);
    //}
    //foreach (var candle in newCandles)
    //{
    //  Assert.Contains(candle, assertedPeriod.Candles);
    //}
  }
  [Fact]
  [Trait("Group", "add cross candles")]
  public void WHEN_new_Date_THEN_exception()
  {
    // Array
    var newCandles = CandleFactory.RandomCandles(5, new DateTime(2020, 1, 3).ToUniversalTime());
    var crossCandles = CandleFactory.RandomCandles(1, new DateTime(2020, 1, 11).ToUniversalTime());

    var extendedCandles = new List<Candle>();
    extendedCandles.AddRange(newCandles);
    extendedCandles.AddRange(crossCandles);

    var newPeriod = LoadedPeriod.BuildNewPeriod(
      baseInstument,
      baseTimeFrame,
      new DateTime(2020, 1, 3).ToUniversalTime(),
      new DateTime(2020, 1, 18).ToUniversalTime(),
      extendedCandles);

    // Act


    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => basePeriod.Extend(newPeriod));
  }

  [Fact]
  [Trait("Group", "add cross candles")]
  public void WHEN_new_values_THEN_exception()
  {
    // Array
    var newCandles = CandleFactory.RandomCandles(5, new DateTime(2020, 1, 3).ToUniversalTime());
    Candle crossCandle1 = new Candle(baseCandles.ElementAt(0).DateTime,
                                     baseCandles.ElementAt(0).OpenStore + 1,
                                     baseCandles.ElementAt(0).HighStore,
                                     baseCandles.ElementAt(0)._LowStore,
                                     baseCandles.ElementAt(0).CloseStore,
                                     baseCandles.ElementAt(0).VolumeStore,
                                     baseCandles.ElementAt(0).TimeFrameId,
                                     baseCandles.ElementAt(0).InstrumentId);
    var crossCandles = new[] { crossCandle1, baseCandles.ElementAt(1) };

    var extendedCandles = new List<Candle>();
    extendedCandles.AddRange(newCandles);
    extendedCandles.AddRange(crossCandles);

    var newPeriod = LoadedPeriod.BuildNewPeriod(
      baseInstument,
      baseTimeFrame,
      new DateTime(2020, 1, 3).ToUniversalTime(),
      new DateTime(2020, 1, 18).ToUniversalTime(),
      extendedCandles);

    // Act


    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => basePeriod.Extend(newPeriod));
  }
  */
}