using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Test.TimeFrameAggregate;

[Collection("Host collection")]
public class ITimeFrameSrv_GetAll_Test : BaseTest
{

  public ITimeFrameSrv_GetAll_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async Task WHEN_request_all_timeframes_THEN_get_list_of_them()
  {
    #region Array
    Logger.LogInformation("Test ARRAY");



    #endregion


    #region Act
    Logger.LogInformation("Test ACT");
    Result<IEnumerable<TimeFrameResponseDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = await usedTimeFrameSrv.GetAllAsync();
    }

    #endregion


    #region Assert
    Logger.LogInformation("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain timeframe", () => Assert.Equal(TimeFrame.ToList().Count(), assertedResult.Value.Count()));
    ExpectGroup("Contain timeframes", () =>
    {
      foreach (var item in TimeFrame.ToList().Select(e => new TimeFrame(e.Id)))
      {
        Logger.LogInformation($"Check TimeFrame {item.Name}");
        Expect("Has dto with correct id", () => assertedResult.Value.Single(e => e.Id == item.Id), out var assertedDto);
        Expect("Correct name", () => Assert.Equal(item.Name, assertedDto.Code));
        Expect("Correct value in seconds", () => Assert.Equal(item.Seconds, assertedDto.Seconds));
      }
    });

    #endregion
  }
}
