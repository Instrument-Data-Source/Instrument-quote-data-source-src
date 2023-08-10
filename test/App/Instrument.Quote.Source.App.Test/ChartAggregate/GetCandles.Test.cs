using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Test.Tools;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.ChartAggregate;

public class GetCandles_Test : BaseTest
{

  public GetCandles_Test(ITestOutputHelper output) : base(output)
  {

  }


  [Fact]
  public async Task WHEN_reques_exist_candles_THEN_get_dataAsync()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await hostFixture.Services.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    var expectedCandles = usedUploadedCandles1.Candles.Where(e => e.DateTime >= fromDt && e.DateTime < untillDt).ToList();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
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

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await hostFixture.Services.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
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

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await hostFixture.Services.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
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

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var usedUploadedCandles2 = await hostFixture.Services.InitChartData(usedInstrument2, TimeFrame.Enum.H1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
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

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
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

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    var fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    var untillDt = usedUploadedCandles1.UntillDate + new TimeSpan(1, 0, 0, 0);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
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

  [Theory]
  [InlineData(false, true)]
  [InlineData(true, false)]
  [InlineData(false, false)]
  public async Task WHEN_request_period_is_not_utc_THEN_get_validation_error(bool fromIsUTC, bool untillIsUTC)
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.InitChartData(usedInstrument1, TimeFrame.Enum.D1);
    DateTime fromDt;
    DateTime untillDt;
    if (fromIsUTC)
      fromDt = usedUploadedCandles1.FromDate + new TimeSpan(1, 0, 0, 0);
    else
      fromDt = new DateTime(usedUploadedCandles1.FromDate.Year, usedUploadedCandles1.FromDate.Month, usedUploadedCandles1.FromDate.Day) + new TimeSpan(1, 0, 0, 0);
    if (untillIsUTC)
      untillDt = usedUploadedCandles1.UntillDate - new TimeSpan(1, 0, 0, 0);
    else
      untillDt = new DateTime(usedUploadedCandles1.UntillDate.Year, usedUploadedCandles1.UntillDate.Month, usedUploadedCandles1.UntillDate.Day) - new TimeSpan(1, 0, 0, 0);

    #endregion

    #region Act
    Logger.LogDebug("Test ACT");



    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");
    Result<IEnumerable<CandleDto>> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      var assertedException = await ExpectTaskAsync<ArgumentException>("Exception", async () =>
        await Assert.ThrowsAsync<ArgumentException>(async () =>
          await usedSrv.GetAsync(usedInstrument1.Id, (int)TimeFrame.Enum.D1, fromDt, untillDt)
        )
      );
      Logger.LogDebug(assertedException.Message);
    }
    #endregion
  }
}