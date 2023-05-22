namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Model;

using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;
public class Candle_Test
{

  public Candle_Test(ITestOutputHelper output)
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
  public void WHEN_correct_data_THEN_correct_entity(int open, int high, int low, int close, int volume)
  {
    // Array
    var dt = new DateTime(2020, 1, 1).ToUniversalTime();

    // Act
    var asserted_candle = new Candle(dateTime: dt,
        openStore: open, closeStore: close,
        highStore: high, lowStore: low,
        volumeStore: volume,
        timeFrameId: (int)TimeFrame.Enum.M,
        instrumentId: 2);

    // Assert
    Assert.Equal(dt, asserted_candle.DateTime);
    Assert.Equal(open, asserted_candle.OpenStore);
    Assert.Equal(high, asserted_candle.HighStore);
    Assert.Equal(low, asserted_candle.LowStore);
    Assert.Equal(close, asserted_candle.CloseStore);
    Assert.Equal(volume, asserted_candle.VolumeStore);
    Assert.Equal(1, asserted_candle.TimeFrameId);
    Assert.Equal(2, asserted_candle.InstrumentId);
  }

  [Fact]
  public void WHEN_dt_not_utc_THEN_error()
  {
    // Array

    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => new Candle(DateTime.Now, 1, 1, 1, 1, 1, 1, 1));
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
  [MemberData(nameof(CorrectData))]
  public void WHEN_wrong_candle_values_THEN_error(int open, int high, int low, int close, int volume)
  {
    // Array

    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() =>
        new Candle(
          new DateTime(2020, 1, 1),
          openStore: open, closeStore: close,
          highStore: high, lowStore: low,
          volumeStore: 1,
          timeFrameEnumId: TimeFrame.Enum.D1,
          instrumentId: 1));
  }


}