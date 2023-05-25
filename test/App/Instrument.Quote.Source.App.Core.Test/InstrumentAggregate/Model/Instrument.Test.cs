using System.Net;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;
using m = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Model;

public class Instument_Constructor_Test
{

  public Instument_Constructor_Test(ITestOutputHelper output)
  {

  }

  [Fact]
  public void WHEN_create_correct_data_THEN_ok()
  {
    // Array
    var expected_name = "Instrument";
    var expected_code = "I1";
    byte expected_PriceDec = 4;
    byte expected_VolDec = 2;
    var expected_Type = 1;
    // Act
    var asserted_ent = new m.Instrument(expected_name, expected_code, expected_PriceDec, expected_VolDec, expected_Type);
    // Assert
    Assert.Equal(expected_name, asserted_ent.Name);
    Assert.Equal(expected_code, asserted_ent.Code);
    Assert.Equal(expected_PriceDec, asserted_ent.PriceDecimalLen);
    Assert.Equal(expected_VolDec, asserted_ent.VolumeDecimalLen);
    Assert.Equal(expected_Type, asserted_ent.InstrumentTypeId);
  }

  [Theory]
  [InlineData("Instrument", null)]
  [InlineData(null, "I1")]
  [InlineData(null, null)]
  public void WHEN_null_THEN_Expection(string expected_name, string expected_code)
  {
    // Array
    byte expected_PriceDec = 4;
    byte expected_VolDec = 2;
    var expected_Type = 1;
    // Act

    // Assert
    Assert.Throws<FluentValidation.ValidationException>(() => new m.Instrument(expected_name, expected_code, expected_PriceDec, expected_VolDec, expected_Type));
  }

  [Fact]
  public void WHEN_incorrect_type_THEN_exception()
  {
    // Array
    var expected_name = "Instrument";
    var expected_code = "I1";
    byte expected_PriceDec = 4;
    byte expected_VolDec = 2;
    var expected_Type = -1;
    // Act

    // Assert
    try
    {
      new m.Instrument(expected_name, expected_code, expected_PriceDec, expected_VolDec, expected_Type);
    }
    catch (System.Exception ex)
    {

      throw;
    }
    Assert.Throws<FluentValidation.ValidationException>(() => new m.Instrument(expected_name, expected_code, expected_PriceDec, expected_VolDec, expected_Type));

  }
}