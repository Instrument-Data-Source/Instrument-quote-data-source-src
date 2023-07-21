namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mapper;
using System.Net;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
public class DecimalToStoreIntCoverter_Test : BaseTest<DecimalToStoreIntCoverter_Test>
{
  DecimalToStoreIntConverter assertedMapper;
  ent.Instrument usedInstrument = MockInstrument.Create(3, 4);
  public DecimalToStoreIntCoverter_Test(ITestOutputHelper output) : base(output)
  {
    assertedMapper = new DecimalToStoreIntConverter(usedInstrument);
  }
  public static IEnumerable<object[]> PriceDecToInt
  {
    get
    {

      yield return new object[] { (decimal)2.23, 2230 };
      yield return new object[] { (decimal)2, 2000 };
      yield return new object[] { (decimal)2.123, 2123 };
      yield return new object[] { (decimal)0.002, 2 };
    }
  }
  [Theory]
  [MemberData(nameof(PriceDecToInt))]
  public void WHEN_convert_decimal_price_to_int_THEN_correct_convertation(decimal usedPrice, int expectedPrice)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedPrice = assertedMapper.PriceToInt(usedPrice);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Price convert correct", () => Assert.Equal(expectedPrice, assertedPrice));

    #endregion
  }

  [Theory]
  [MemberData(nameof(PriceDecToInt))]
  public void WHEN_convert_int_price_to_decimal_THEN_correct_convertation(decimal expectedPrice, int usedPrice)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedPrice = assertedMapper.PriceToDecimal(usedPrice);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Price convert correct", () => Assert.Equal(expectedPrice, assertedPrice));

    #endregion
  }

  [Fact]
  public void WHEN_convert_price_decimal_with_too_long_divider_THEN_exception()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Exception when decimal to int", () => Assert.Throws<ArgumentOutOfRangeException>(() => assertedMapper.PriceToInt((decimal)1.2333)));

    #endregion
  }

  public static IEnumerable<object[]> VolumeDecToInt
  {
    get
    {

      yield return new object[] { (decimal)2.23, 22300 };
      yield return new object[] { (decimal)2, 20000 };
      yield return new object[] { (decimal)2.1234, 21234 };
      yield return new object[] { (decimal)0.0002, 2 };
    }
  }
  [Theory]
  [MemberData(nameof(VolumeDecToInt))]
  public void WHEN_convert_decimal_volume_to_int_THEN_correct_convertation(decimal usedVolume, int expectedVolume)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedVolume = assertedMapper.VolumeToInt(usedVolume);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Volume convert correct", () => Assert.Equal(expectedVolume, assertedVolume));

    #endregion
  }

  [Theory]
  [MemberData(nameof(VolumeDecToInt))]
  public void WHEN_convert_int_volume_to_decimal_THEN_correct_convertation(decimal expectedVolume, int usedVolume)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedVolume = assertedMapper.VolumeToDecimal(usedVolume);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Volume convert correct", () => Assert.Equal(expectedVolume, assertedVolume));

    #endregion
  }

  [Fact]
  public void WHEN_convert_volume_decimal_with_too_long_divider_THEN_exception()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Exception when decimal to int", () => Assert.Throws<ArgumentOutOfRangeException>(() => assertedMapper.VolumeToInt((decimal)1.23333)));

    #endregion
  }
}