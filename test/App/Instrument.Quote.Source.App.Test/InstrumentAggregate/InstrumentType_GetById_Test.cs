using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.InstrumentAggregate;

public class InstrumentType_GetById_Test : BaseTest
{

  public InstrumentType_GetById_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_by_id_THEN_get_correct_answer()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<InstrumentTypeResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IInstrumentTypeSrv>();
      assertedResult = await usedSrv.GetByAsync(1);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain correct name", () => Assert.Equal("Currency", assertedResult.Value.Name));
    Expect("Result contain correct id", () => Assert.Equal(1, assertedResult.Value.Id));

    #endregion
  }

  [Fact]
  public async void WHEN_request_by_wrong_id_THEN_notFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<InstrumentTypeResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IInstrumentTypeSrv>();
      assertedResult = await usedSrv.GetByAsync(-1);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Result error is correct", () =>
    {
      Logger.LogDebug(string.Join("\n", assertedResult.Errors));
      Expect("Result contain 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error is InstrumnentType", () => Assert.Equal(nameof(ent.InstrumentType), assertedError));
    });

    #endregion
  }
}
