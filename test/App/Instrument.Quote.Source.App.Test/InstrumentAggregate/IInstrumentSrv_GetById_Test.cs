namespace Instrument.Quote.Source.App.Test.InstrumentAggregate;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Ardalis.Result;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;

public class IInstrumentSrv_GetById_Test : BaseDbTest
{
  public IInstrumentSrv_GetById_Test(ITestOutputHelper output) : base(output)
  {

  }
  [Fact]
  public async void WHEN_request_by_id_THEN_get_correct_instrument()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var expectedDto1, var expectedDto2) = await this.InitInstrumentData();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<InstrumentResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedInstrumentSrv = sp.GetRequiredService<IInstrumentSrv>();
      assertedResult = await usedInstrumentSrv.GetByAsync(expectedDto1.Id);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");
    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result return correct dto", () => Assert.Equal(expectedDto1, assertedResult.Value));

    #endregion
  }

  [Fact]
  public async void WHEN_request_by_wrong_id_THEN_get_notFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var expectedDto1, var expectedDto2) = await this.InitInstrumentData();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<InstrumentResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedInstrumentSrv = sp.GetRequiredService<IInstrumentSrv>();
      assertedResult = await usedInstrumentSrv.GetByAsync(123213);
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