using InsonusK.Xunit.ExpectationsTest;
using NSubstitute;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Test.Tools;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Microsoft.Extensions.DependencyInjection;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Ardalis.Result;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Microsoft.EntityFrameworkCore;
namespace Instrument.Quote.Source.App.Test.ChartAggregate;


public class UploadChart_Test : BaseDbTest
{

  public UploadChart_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_upload_data_THEN_data_saved_in_db()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var expectedFrom = new DateTime(2020, 3, 1).ToUniversalTime();
    var expectedUntill = new DateTime(2020, 4, 1).ToUniversalTime();
    var expectedCandles = new MockCandleDtoFactory().CreateCandleDtos(expectedFrom, expectedUntill);
    var uploadedData = new UploadedCandlesDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      Candles = expectedCandles
    };

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<int> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.AddAsync(usedInstrument1.Id, (int)TimeFrame.Enum.D1, uploadedData);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result value equal to count of candles", () => Assert.Equal(expectedCandles.Count(), assertedResult.Value));
    ExpectGroup("Instrument exist in repository", async () =>
    {
      using (var assert_scope = this.global_sp.CreateScope())
      {
        var sp = assert_scope.ServiceProvider;
        var chartRep = sp.GetRequiredService<IReadRepository<Chart>>();
        var assertedChart = chartRep.Table.Include(e => e.Candles).Single(e => e.InstrumentId == usedInstrument1.Id && e.TimeFrameId == (int)TimeFrame.Enum.D1);
        ExpectGroup("Chart is correct saved", () =>
        {
          Expect("From is correct", () => Assert.Equal(expectedFrom, assertedChart.FromDate));
          Expect("Untill is correct", () => Assert.Equal(expectedUntill, assertedChart.UntillDate));
        });
        ExpectGroup("Candles is uploaded", () =>
        {
          Expect("Count of candles is correct", () => Assert.Equal(expectedCandles.Count(), assertedChart.Candles.Count()));
          Expect("Each candle saved", () =>
          {
            foreach (var expectedCandle in expectedCandles)
            {
              Assert.Single(assertedChart.Candles.Where(e => e.DateTime == expectedCandle.DateTime));
            }
          });
        });
      }
    });
    #endregion
  }

  [Fact]
  public async void WHEN_upload_addition_data_THEN_save_in_db()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1);

    var expectedFrom = usedUploadedCandles.UntillDate;
    var expectedUntill = usedUploadedCandles.UntillDate + new TimeSpan(60, 0, 0, 0);
    var expectedCandles = new MockCandleDtoFactory().CreateCandleDtos(expectedFrom, expectedUntill);
    var uploadedData = new UploadedCandlesDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      Candles = expectedCandles
    };

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<int> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.AddAsync(usedInstrument1.Id, (int)TimeFrame.Enum.D1, uploadedData);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result value equal to count of candles", () => Assert.Equal(expectedCandles.Count(), assertedResult.Value));
    ExpectGroup("Instrument exist in repository", async () =>
    {
      using (var assert_scope = this.global_sp.CreateScope())
      {
        var sp = assert_scope.ServiceProvider;
        var chartRep = sp.GetRequiredService<IReadRepository<Chart>>();
        var assertedChart = chartRep.Table.Include(e => e.Candles).Single(e => e.InstrumentId == usedInstrument1.Id && e.TimeFrameId == (int)TimeFrame.Enum.D1);
        ExpectGroup("Chart is correct saved", () =>
        {
          Expect("From is correct", () => Assert.Equal(usedUploadedCandles.FromDate, assertedChart.FromDate));
          Expect("Untill is correct", () => Assert.Equal(expectedUntill, assertedChart.UntillDate));
        });
        ExpectGroup("Candles is uploaded", () =>
        {
          Expect("Count of candles is correct", () => Assert.Equal(expectedCandles.Count() + usedUploadedCandles.Candles.Count(), assertedChart.Candles.Count()));
          Expect("Each candle saved", () =>
          {
            foreach (var expectedCandle in expectedCandles)
            {
              Assert.Single(assertedChart.Candles.Where(e => e.DateTime == expectedCandle.DateTime));
            }
            foreach (var expectedCandle in usedUploadedCandles.Candles)
            {
              Assert.Single(assertedChart.Candles.Where(e => e.DateTime == expectedCandle.DateTime));
            }
          });
        });
      }
    });

    #endregion
  }

  [Fact]
  public async void WHEN_upload_data_for_unkonwn_timeframe_THEN_notFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var expectedFrom = new DateTime(2020, 3, 1).ToUniversalTime();
    var expectedUntill = new DateTime(2020, 4, 1).ToUniversalTime();
    var expectedCandles = new MockCandleDtoFactory().CreateCandleDtos(expectedFrom, expectedUntill);
    var uploadedData = new UploadedCandlesDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      Candles = expectedCandles
    };

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<int> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.AddAsync(usedInstrument1.Id, -1, uploadedData);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Result errors is correct", () =>
    {
      Expect("Has single error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error is instrument", () => Assert.Equal(nameof(TimeFrame), assertedError));
    });
    #endregion
  }
  [Fact]
  public async void WHEN_upload_data_for_unkonwn_instrument_THEN_notFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var expectedFrom = new DateTime(2020, 3, 1).ToUniversalTime();
    var expectedUntill = new DateTime(2020, 4, 1).ToUniversalTime();
    var expectedCandles = new MockCandleDtoFactory().CreateCandleDtos(expectedFrom, expectedUntill);
    var uploadedData = new UploadedCandlesDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      Candles = expectedCandles
    };

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<int> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.AddAsync(-1, (int)TimeFrame.Enum.D1, uploadedData);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Result errors is correct", () =>
    {
      Expect("Has single error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error is instrument", () => Assert.Equal(nameof(ent.Instrument), assertedError));
    });
    #endregion
  }
}