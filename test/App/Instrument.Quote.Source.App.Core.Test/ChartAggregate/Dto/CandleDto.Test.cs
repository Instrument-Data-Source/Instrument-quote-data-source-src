namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Dto;

using System.Net;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
public class CandleDto_IsAnyDecimalPartLonger_Test : BaseTest<CandleDto_IsAnyDecimalPartLonger_Test>
{

  public CandleDto_IsAnyDecimalPartLonger_Test(ITestOutputHelper output) : base(output)
  {

  }
  [Fact]
  public void WHEN_decimal_is_correct_THEN_true()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var usedInstrument = MockInstrument.Create(3, 4);
    var assertedDto = new CandleDto()
    {
      Open = 1.123m,
      High = 2.423m,
      Low = 3.34m,
      Close = 2m,
      Volume = 3.3456m
    };
    var usedChecker = new DecimalToStoreIntConverter(usedInstrument);
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = assertedDto.IsDecimalPartFit(usedChecker);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Check passed successfully", () => Assert.True(assertedResult));

    #endregion
  }

  public static IEnumerable<object[]> IncorrectDto
  {
    get
    {

      yield return new object[] {
        new CandleDto()
          {
            Open = 1.1233m,
            High = 2.423m,
            Low = 3.34m,
            Close = 2m,
            Volume = 3.3456m
          }
      };

      yield return new object[] {
        new CandleDto()
          {
            Open = 1.123m,
            High = 2.4233m,
            Low = 3.34m,
            Close = 2m,
            Volume = 3.3456m
          }
      };
      yield return new object[] {
        new CandleDto()
          {
            Open = 1.123m,
            High = 2.423m,
            Low = 3.3432m,
            Close = 2m,
            Volume = 3.3456m
          }
      };
      yield return new object[] {
        new CandleDto()
          {
            Open = 1.123m,
            High = 2.423m,
            Low = 3.34m,
            Close = 2.4343m,
            Volume = 3.3456m
          }
      };
      yield return new object[] {
        new CandleDto()
          {
            Open = 1.123m,
            High = 2.423m,
            Low = 3.34m,
            Close = 2m,
            Volume = 3.34556m
          }
      };
      yield return new object[] {
        new CandleDto()
          {
            Open = 1.1233m,
            High = 2.4233m,
            Low = 3.33434m,
            Close = 2.43434m,
            Volume = 3.34556m
          }
      };
    }
  }
  [Theory]
  [MemberData(nameof(IncorrectDto))]
  public void WHEN_decimal_is_incorrect_THEN_false(CandleDto assertedDto)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var usedInstrument = MockInstrument.Create(3, 4);
    var usedChecker = new DecimalToStoreIntConverter(usedInstrument);
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = assertedDto.IsDecimalPartFit(usedChecker);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Check passed un successfully", () => Assert.False(assertedResult));

    #endregion
  }
}