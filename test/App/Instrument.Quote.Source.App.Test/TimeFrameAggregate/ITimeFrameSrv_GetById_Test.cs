using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Test.TimeFrameAggregate;

public class ITimeFrameSrv_GetById_Test : BaseDbTest
{

  public ITimeFrameSrv_GetById_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_info_of_TimeFrame_by_exist_id_THEN_get_timeframe()
  {
    #region Array
    Logger.LogInformation("Test ARRAY");
    var expectedData = TimeFrame.Enum.D1.ToEntity();

    #endregion


    #region Act
    Logger.LogInformation("Test ACT");
    Result<TimeFrameResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = await usedTimeFrameSrv.GetByIdAsync(expectedData.Id);
    }

    #endregion


    #region Assert
    Logger.LogInformation("Test ASSERT");

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
  public async void WHEN_request_incorrect_id_THEN_return_not_found_result()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<TimeFrameResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = await usedTimeFrameSrv.GetByIdAsync(9989898);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Result error is correct", () =>
    {
      Expect("Result contain 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error is TimeFrame", () => Assert.Equal(nameof(TimeFrame), assertedError));
    });

    #endregion
  }

}
