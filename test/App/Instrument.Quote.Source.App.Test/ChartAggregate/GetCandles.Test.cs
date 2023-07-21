using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.ChartAggregate;

public class GetCandles_Test : BaseDbTest
{

  public GetCandles_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async Task WHEN_reques_exist_candles_THEN_get_dataAsync()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    var expectedCandles = usedUploadedCandles1.Candles.Where(e => e.DateTime >= fromDt && e.DateTime < untillDt).ToList();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, (int)TimeFrame.Enum.D1, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Result candles is correct", () =>
    {
      Expect("Return correct count of candles", () => Assert.Equal(expectedCandles.Count(), assertedResult.Value.Count()));

      Expect("Contain all candles", () => Assert.True(expectedCandles.All(e => assertedResult.Value.ToList().Contains(e))));
    });

    #endregion
  }

  [Fact]
  public async Task WHEN_request_not_exist_instrument_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(-1, (int)TimeFrame.Enum.D1, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    Expect("Reslut errors is correct", () =>
    {
      Expect("Expect 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Expect Instrument error", () => Assert.Equal(nameof(ent.Instrument), assertedError));
    });

    #endregion
  }

  [Fact]
  public async Task WHEN_request_not_exist_timeframe_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, 99, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    Expect("Reslut errors is correct", () =>
    {
      Expect("Expect 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Expect TimeFrame error", () => Assert.Equal(nameof(TimeFrame), assertedError));
    });

    #endregion
  }

  [Fact]
  public async Task WHEN_request_not_exist_instrument_and_timeframe_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(-1, 99, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    Expect("Reslut errors is correct", () =>
    {
      Expect("Expect 2 error", () => Assert.Equal(2, assertedResult.Errors.Count()));
      Expect("Expect Instrument error", () => Assert.True(assertedResult.Errors.Contains(nameof(ent.Instrument))));
      Expect("Expect TimeFrame error", () => Assert.True(assertedResult.Errors.Contains(nameof(TimeFrame))));
    });

    #endregion
  }

  [Fact]
  public async Task WHEN_request_not_exist_chart_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument2.Id, (int)TimeFrame.Enum.D1, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    Expect("Reslut errors is correct", () =>
    {
      Expect("Expect 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Expect Instrument error", () => Assert.Equal(nameof(Chart), assertedError));
    });

    #endregion
  }

  [Fact]
  public async Task WHEN_request_not_loaded_period_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate + new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, (int)TimeFrame.Enum.D1, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    Expect("Reslut errors is correct", () =>
    {
      Expect("Expect 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Expect Instrument error", () => Assert.Equal(nameof(Candle), assertedError));
    });

    #endregion
  }
}