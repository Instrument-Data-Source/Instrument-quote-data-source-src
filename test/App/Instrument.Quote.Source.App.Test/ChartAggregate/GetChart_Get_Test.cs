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

public class GetChart_Get_Test : BaseTest
{

  public GetChart_Get_Test(ITestOutputHelper output) : base(output)
  {

  }
  [Fact]
  public async void WHEN_request_charts_of_instrument_THEN_get_exist_charts()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await hostFixture.Services.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<ChartDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IChartSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Result chart is correct", () =>
    {
      Expect("Return 2 chart", () => Assert.Equal(1, assertedResult.Value.Count()));


      Expect("Chart 1 is correct", () =>
      {
        Assert.NotNull(assertedResult.Value.SingleOrDefault(e => e.InstrumentId == usedInstrument1.Id &&
                                    e.TimeFrameId == (int)TimeFrame.Enum.D1 &&
                                    e.FromDate == usedUploadedCandles1.FromDate &&
                                    e.UntillDate == usedUploadedCandles1.UntillDate));
      });
    });

    #endregion
  }

  [Fact]
  public async void WHEN_request_charts_of_unknown_instrument_THEN_notFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await hostFixture.Services.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<ChartDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IChartSrv>();
      assertedResult = await usedSrv.GetAsync(-1);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Result error is correct", () =>
    {
      Expect("One error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error is Instrument", () => Assert.Equal(nameof(ent.Instrument), assertedError));
    });

    #endregion
  }

  [Fact]
  public async void WHEN_request_charts_of_instrument_with_no_chart_THEN_empty_list()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<ChartDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IChartSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument2.Id);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Return 0 chart", () => Assert.Equal(0, assertedResult.Value.Count()));

    #endregion
  }
}