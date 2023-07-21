using InsonusK.Xunit.ExpectationsTest;
using NSubstitute;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Dto;


public class UploadedCandlesDto_Validation_Test : ExpectationsTestBase
{

  public UploadedCandlesDto_Validation_Test(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {

  }

  [Fact]
  public void WHEN_Dates_in_candle_out_perid_THEN_invalid_error()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var usedFromDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var usedUntillDt = new DateTime(2020, 3, 1).ToUniversalTime();

    var mockCandleFactory = new MockCandleFactory();
    var usedCandles = mockCandleFactory.CreateCandleDtos(usedFromDt, usedUntillDt).ToList();
    var wrongCandles = mockCandleFactory.CreateCandleDtos(2, new DateTime(2020, 3, 3).ToUniversalTime());
    usedCandles.AddRange(wrongCandles);

    var assertedDto = new UploadedCandlesDto()
    {
      FromDate = new DateTime(2020, 1, 1).ToUniversalTime(),
      UntillDate = new DateTime(2020, 3, 1).ToUniversalTime(),
      Candles = usedCandles
    };

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    var context = new ValidationContext(assertedDto);
    context.MemberName = nameof(UploadedCandlesDto.Candles);
    var assertedResults = new List<ValidationResult>();
    var assertedIsValid = Validator.TryValidateProperty(assertedDto.Candles, context, assertedResults);

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("DTO is not valid", () => Assert.False(assertedIsValid));
    Expect("Result contain one error", () => Assert.Single(assertedResults), out var assertedAbsResult);
    Expect("Result is extendet type", () => Assert.IsType<ValidationResultExtended>(assertedAbsResult), out var assertedResult);
    Expect("Result contain 2 sub erros", () => Assert.Equal(2, assertedResult.SubResult!.Count()));
    Logger.LogInformation(assertedResult.ToString());

    #endregion
  }
}