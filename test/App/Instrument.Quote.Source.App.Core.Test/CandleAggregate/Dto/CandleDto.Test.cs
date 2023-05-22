namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Dto;
using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;
public class CandleDto_Test

{

  public CandleDto_Test(ITestOutputHelper output)

  {

  }

  [Fact]
  public void WHEN_give_equal_THEN_return_true()
  {
    // Array
    var c1 = new CandleDto() { DateTime = new DateTime(2000, 1, 1), Open = 5, High = 10, Low = 1, Close = 6, Volume = 2 };
    var c2 = new CandleDto() { DateTime = new DateTime(2000, 1, 1), Open = 5, High = 10, Low = 1, Close = 6, Volume = 2 };
    // Act

    // Assert
    Assert.Equal(c1, c2);
  }

  [Fact]
  public void WHEN_give_non_equal_THEN_return_false()
  {
    // Array
    var c1 = new CandleDto() { DateTime = new DateTime(2000, 1, 1), Open = 5, High = 10, Low = 1, Close = 6, Volume = 2 };
    var c2 = new CandleDto() { DateTime = new DateTime(2000, 1, 1), Open = 5, High = 10, Low = 1, Close = 775, Volume = 2 };
    // Act

    // Assert
    Assert.NotEqual(c1, c2);
  }
}