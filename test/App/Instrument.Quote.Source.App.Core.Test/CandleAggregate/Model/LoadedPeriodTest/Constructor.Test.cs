namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Model.LoadedPeriodTest;

using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;
public class Constructor_Test
{
/*
  public Constructor_Test(ITestOutputHelper output)
  {

  }

  [Fact]
  public void WHEN_create_entity_THEN_check_from_dt_and_untill_dt_is_UTC()
  {
    // Array
    var usingInstrument = new ent.Instrument("test1", "t1", 2, 2, 1);
    var usingTf = new TimeFrame(TimeFrame.Enum.D1);
    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => new LoadedPeriod(usingInstrument, usingTf, new DateTime(2020, 1, 1), new DateTime(2020, 1, 11).ToUniversalTime()));
    Assert.Throws<FluentValidation.ValidationException>(() => new LoadedPeriod(usingInstrument, usingTf, new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 1, 11)));
    Assert.Throws<FluentValidation.ValidationException>(() => new LoadedPeriod(usingInstrument, usingTf, new DateTime(2020, 1, 1), new DateTime(2020, 1, 11)));
  }

  [Fact]
  public void WHEN_create_entity_THEN_check_from_date_LE_untill_date()
  {
    // Array
    var usingInstrument = new ent.Instrument("test1", "t1", 2, 2, 1);
    var usingTf = new TimeFrame(TimeFrame.Enum.D1);
    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => new LoadedPeriod(usingInstrument, usingTf, new DateTime(2020, 1, 5).ToUniversalTime(), new DateTime(2020, 1, 5).ToUniversalTime()));
    Assert.Throws<FluentValidation.ValidationException>(() => new LoadedPeriod(usingInstrument, usingTf, new DateTime(2020, 1, 5).ToUniversalTime(), new DateTime(2020, 1, 2).ToUniversalTime()));

  }

  [Fact]
  public void WHEN_add_candles_THEN_check_that_they_in_range()
  {
    // Array
    var usingInstrumentId = new ent.Instrument("test1", "t1", 2, 2, 1);
    var usingTf = new TimeFrame(TimeFrame.Enum.D1);
    var usingFromDt = new DateTime(2020, 1, 5).ToUniversalTime();
    var usingUntillDt = new DateTime(2020, 1, 11).ToUniversalTime();
    // Act


    // Assert
    LoadedPeriod.BuildNewPeriod(usingInstrumentId, usingTf, usingFromDt, usingUntillDt, new[] { BuildCandle(new DateTime(2020, 1, 5), 0, (int)TimeFrame.Enum.D1), BuildCandle(new DateTime(2020, 1, 10), 0, (int)TimeFrame.Enum.D1) });
    Assert.Throws<FluentValidation.ValidationException>(() =>
            LoadedPeriod.BuildNewPeriod(usingInstrumentId, usingTf, usingFromDt, usingUntillDt,
                                        new[] { BuildCandle(new DateTime(2020, 1, 4), 0, (int)TimeFrame.Enum.D1) }));
    Assert.Throws<FluentValidation.ValidationException>(() =>
            LoadedPeriod.BuildNewPeriod(usingInstrumentId, usingTf, usingFromDt, usingUntillDt,
                                         new[] { BuildCandle(new DateTime(2020, 1, 11), 0, (int)TimeFrame.Enum.D1) }));
    Assert.Throws<FluentValidation.ValidationException>(() =>
            LoadedPeriod.BuildNewPeriod(usingInstrumentId, usingTf, usingFromDt, usingUntillDt,
                                          new[] { BuildCandle(new DateTime(2020, 1, 4), 0, (int)TimeFrame.Enum.D1), BuildCandle(new DateTime(2020, 1, 6), 0, 1) }));
    Assert.Throws<FluentValidation.ValidationException>(() =>
            LoadedPeriod.BuildNewPeriod(usingInstrumentId, usingTf, usingFromDt, usingUntillDt,
                                          new[] { BuildCandle(new DateTime(2020, 1, 6), 0, (int)TimeFrame.Enum.D1), BuildCandle(new DateTime(2020, 1, 11), 0, 1) }));
  }

  [Fact]
  public void WHEN_add_candles_THEN_check_that_no_duplicate_date()
  {
    // Array

    var usingInstrumentId = new ent.Instrument("test1", "t1", 2, 2, 1);
    var usingTf = new TimeFrame(TimeFrame.Enum.D1);
    var usingFromDt = new DateTime(2020, 1, 5).ToUniversalTime();
    var usingUntillDt = new DateTime(2020, 1, 11).ToUniversalTime();

    // Act


    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() =>
                  LoadedPeriod.BuildNewPeriod(usingInstrumentId, usingTf, usingFromDt, usingUntillDt,
                        new[] {
                          BuildCandle(new DateTime(2020, 1, 5), 0, (int)TimeFrame.Enum.D1),
                          BuildCandle(new DateTime(2020, 1, 10), 0, (int)TimeFrame.Enum.D1),
                          BuildCandle(new DateTime(2020, 1, 5), 0, (int)TimeFrame.Enum.D1) }));
  }

  [Fact]
  public void WHEN_add_candles_THEN_check_that_tf_id_and_indicator_id()
  {
    // Array
    var usingInstrumentId = new ent.Instrument("test1", "t1", 2, 2, 1);
    var usingTf = new TimeFrame(TimeFrame.Enum.D1);
    var usingFromDt = new DateTime(2020, 1, 5).ToUniversalTime();
    var usingUntillDt = new DateTime(2020, 1, 11).ToUniversalTime();
    //var loadedP = new LoadedPeriod(0, (int)TimeFrame.Enum.D1, new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 1, 11).ToUniversalTime());

    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() =>
                  LoadedPeriod.BuildNewPeriod(usingInstrumentId, usingTf, usingFromDt, usingUntillDt,
                          new[] { BuildCandle(new DateTime(2020, 1, 1), 0, (int)TimeFrame.Enum.M) }));
    Assert.Throws<FluentValidation.ValidationException>(() =>
                  LoadedPeriod.BuildNewPeriod(usingInstrumentId, usingTf, usingFromDt, usingUntillDt,
                          new[] { BuildCandle(new DateTime(2020, 1, 1), 1, (int)TimeFrame.Enum.D1) }));
    //Assert.Equal(1, loadedP.Candles.Count());
  }

  private static Candle BuildCandle(DateTime dt, int instrumentId, int timeFrameId)
  {
    var rnd = new Random();
    var price = rnd.Next(10, 100);
    return new Candle(
        dt.ToUniversalTime(),
        price,
        price + 10,
        price - 9,
        price + 4,
        price / 2,
        timeFrameId: timeFrameId,
        instrumentId: instrumentId
         );
  }
  */
}