using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.ChartAggregate;

public class GetChart_GetAll_Test : BaseDbTest
{

  public GetChart_GetAll_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_all_charts_THEN_get_exist_charts()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<ChartDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IChartSrv>();
      assertedResult = await usedSrv.GetAllAsync();
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Result chart is correct", () =>
    {
      Expect("Return 2 chart", () => Assert.Equal(2, assertedResult.Value.Count()));


      Expect("Chart 1 is correct", () =>
      {
        Assert.NotNull(assertedResult.Value.SingleOrDefault(e => e.InstrumentId == usedInstrument1.Id &&
                                    e.TimeFrameId == (int)TimeFrame.Enum.D1 &&
                                    e.FromDate == usedUploadedCandles1.FromDate &&
                                    e.UntillDate == usedUploadedCandles1.UntillDate));
      });
      Expect("Chart 2 is correct", () =>
      {
        Assert.NotNull(assertedResult.Value.SingleOrDefault(e => e.InstrumentId == usedInstrument2.Id &&
                                    e.TimeFrameId == (int)TimeFrame.Enum.H1 &&
                                    e.FromDate == usedUploadedCandles2.FromDate &&
                                    e.UntillDate == usedUploadedCandles2.UntillDate));
      });
    });

    #endregion
  }
}
