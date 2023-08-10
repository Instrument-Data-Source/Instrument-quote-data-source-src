namespace Instrument.Quote.Source.App.Test.InstrumentAggregate;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Ardalis.Result;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;

public class IInstrumentSrv_GetByCode_Test : BaseTest
{
  public IInstrumentSrv_GetByCode_Test(ITestOutputHelper output) : base(output)
  {

  }
  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async Task WHEN_request_instrument_by_code_THEN_get_exist_instrument(bool changeCase)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var expectedDto1, var expectedDto2) = await hostFixture.Services.AddMockInstrumentData();
    var usedCode = expectedDto1.Code;
    if (changeCase)
    {
      if (usedCode.ToLower().Equals(usedCode))
        throw new ApplicationException("Test case must change case of code");
      usedCode = usedCode.ToLower();
    }
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<InstrumentResponseDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedInstrumentSrv = sp.GetRequiredService<IInstrumentSrv>();
      assertedResult = await usedInstrumentSrv.GetByAsync(usedCode);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");
    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result return single dto", () => Assert.Single(assertedResult.Value), out var assertedDto);
    Expect("Result return correct dto", () => Assert.Equal(expectedDto1, assertedDto));

    #endregion
  }

  [Fact]
  public async void WHEN_request_instrument_by_wrong_code_THEN_get_NotFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var expectedDto1, var expectedDto2) = await hostFixture.Services.AddMockInstrumentData();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<InstrumentResponseDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedInstrumentSrv = sp.GetRequiredService<IInstrumentSrv>();
      assertedResult = await usedInstrumentSrv.GetByAsync("eou");
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Result error", () =>
    {
      Expect("Contain single error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error is Instrument", () => Assert.Equal(nameof(ent.Instrument), assertedError));
    });

    #endregion
  }
}
