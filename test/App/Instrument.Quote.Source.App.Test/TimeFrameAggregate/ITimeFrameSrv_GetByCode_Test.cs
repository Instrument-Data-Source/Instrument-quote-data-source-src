using System.Net;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Test.TimeFrameAggregate;

[Collection("Host collection")]
public class ITimeFrameSrv_GetByCode_Test : BaseTest
{

  public ITimeFrameSrv_GetByCode_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_by_exist_Code_THEN_get_timeframe()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    var expectedData = TimeFrame.Enum.D1.ToEntity();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");
    Result<TimeFrameResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = await usedTimeFrameSrv.GetByCodeAsync(expectedData.Name);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Return correct DTO", () =>
    {
      Expect("ID is correct", () => Assert.Equal(expectedData.Id, assertedResult.Value.Id));
      Expect("Code is correct", () => Assert.Equal(expectedData.Name, assertedResult.Value.Code));
      Expect("Seconds is correct", () => Assert.Equal(expectedData.Seconds, assertedResult.Value.Seconds));
    });

    #endregion
  }

  [Fact]
  public async void WHEN_request_incorrect_code_THEN_return_not_found_result()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<TimeFrameResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = await usedTimeFrameSrv.GetByCodeAsync("9989898");
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Result error is correct", () =>
    {
      Logger.LogDebug(string.Join("\n", assertedResult.Errors));
      Expect("Result contain 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error is TimeFrame", () => Assert.Equal(nameof(TimeFrame), assertedError));
    });
    #endregion
  }
}
