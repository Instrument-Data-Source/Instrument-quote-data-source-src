using Ardalis.Result;
using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;

namespace Instrument.Quote.Source.App.Test.ChartAggregate;
public class GetJoinedCandles_Test : BaseDbTest
{

  public GetJoinedCandles_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_joined_chart_THEN_get_correct_data()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1);
    var untillDt = new DateTime(2020, 10, 1);
    var expectFirstJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= fromDt && c.DateTime < new DateTime(2020, 3, 1)).ToArray();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("8 candle", () => Assert.Equal(8, assertedResult.Value.Count()));
    Expect("First candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.ElementAt(0);
      Expect("Start DT is correct", () => Assert.Equal(fromDt, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectFirstJoindeCandles.Select(c => c.High).Max(), assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectFirstJoindeCandles.Select(c => c.Low).Min(), assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectFirstJoindeCandles[0].Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectFirstJoindeCandles[expectFirstJoindeCandles.Count() - 1].Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectFirstJoindeCandles.Select(c => c.Volume).Sum(), assertedCandle.Volume));
      Expect("IsLast eq true", () => Assert.True(assertedCandle.IsLast));
      Expect("Step DateTime is Correct", () => Assert.Equal(expectFirstJoindeCandles[expectFirstJoindeCandles.Count() - 1].DateTime, assertedCandle.StepDateTime));
    });

    #endregion
  }

  [Fact]
  public async void WHEN_request_joined_chart_with_intermediate_candle_THEN_get_correct_data()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1);
    var untillDt = new DateTime(2020, 9, 15);
    var expectFirstJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= fromDt && c.DateTime < new DateTime(2020, 3, 1)).ToArray();
    var expectLastJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= new DateTime(2020, 9, 1) && c.DateTime < new DateTime(2020, 10, 1)).ToArray();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("8 candle", () => Assert.Equal(8, assertedResult.Value.Count()));
    Expect("First candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.ElementAt(0);
      var expectedCandle = expectFirstJoindeCandles.ElementAt(0);
      Expect("Start DT is correct", () => Assert.Equal(fromDt, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectedCandle.High, assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectedCandle.Low, assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectedCandle.Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectedCandle.Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectedCandle.Volume, assertedCandle.Volume));
      Expect("Is Last is false", () => Assert.False(assertedCandle.IsLast));
      Expect("Step DateTime is Correct", () => Assert.Equal(expectFirstJoindeCandles.ElementAt(0).DateTime, assertedCandle.StepDateTime));
    });
    Expect("First candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.ElementAt(1);
      var expectedCandle = new CandleDto()
      {
        DateTime = expectFirstJoindeCandles.ElementAt(0).DateTime,
        High = expectFirstJoindeCandles.Where((dto, idx) => idx < 2).Select(c => c.High).Max(),
        Low = expectFirstJoindeCandles.Where((dto, idx) => idx < 2).Select(c => c.Low).Min(),
        Open = expectFirstJoindeCandles.ElementAt(0).Open,
        Close = expectFirstJoindeCandles.ElementAt(1).Close,
        Volume = expectFirstJoindeCandles.Where((dto, idx) => idx < 2).Select(c => c.Volume).Sum()
      };
      Expect("Start DT is correct", () => Assert.Equal(fromDt, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectedCandle.High, assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectedCandle.Low, assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectedCandle.Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectedCandle.Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectedCandle.Volume, assertedCandle.Volume));
      Expect("Is Last is false", () => Assert.False(assertedCandle.IsLast));
      Expect("Step DateTime is Correct", () => Assert.Equal(expectFirstJoindeCandles.ElementAt(1).DateTime, assertedCandle.StepDateTime));
    });
    Expect("Last candle in group is correct", () =>
    {
      var assertedCandle = assertedResult.Value.ElementAt(expectFirstJoindeCandles.Count() - 1);
      var expectedCandle = new CandleDto()
      {
        DateTime = expectFirstJoindeCandles.ElementAt(0).DateTime,
        High = expectFirstJoindeCandles.Select(c => c.High).Max(),
        Low = expectFirstJoindeCandles.Select(c => c.Low).Min(),
        Open = expectFirstJoindeCandles.ElementAt(0).Open,
        Close = expectFirstJoindeCandles.ElementAt(expectFirstJoindeCandles.Count() - 1).Close,
        Volume = expectFirstJoindeCandles.Select(c => c.Volume).Sum()
      };
      Expect("Start DT is correct", () => Assert.Equal(fromDt, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectedCandle.High, assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectedCandle.Low, assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectedCandle.Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectedCandle.Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectedCandle.Volume, assertedCandle.Volume));
      Expect("Is Last is True", () => Assert.True(assertedCandle.IsLast));
      Expect("Step DateTime is Correct", () => Assert.Equal(expectFirstJoindeCandles[expectFirstJoindeCandles.Count() - 1].DateTime, assertedCandle.StepDateTime));
    });

    Expect("Last candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.ElementAt(assertedResult.Value.Count() - 1);
      var expectedCandle = new CandleDto()
      {
        DateTime = expectLastJoindeCandles.ElementAt(0).DateTime,
        High = expectLastJoindeCandles.Select(c => c.High).Max(),
        Low = expectLastJoindeCandles.Select(c => c.Low).Min(),
        Open = expectLastJoindeCandles.ElementAt(0).Open,
        Close = expectLastJoindeCandles.ElementAt(expectLastJoindeCandles.Count() - 1).Close,
        Volume = expectLastJoindeCandles.Select(c => c.Volume).Sum()
      };
      Expect("Start DT is correct", () => Assert.Equal(fromDt, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectedCandle.High, assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectedCandle.Low, assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectedCandle.Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectedCandle.Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectedCandle.Volume, assertedCandle.Volume));
      Expect("Is Last is True", () => Assert.False(assertedCandle.IsLast));
      Expect("Step DateTime is Correct", () => Assert.Equal(expectLastJoindeCandles[expectLastJoindeCandles.Count() - 1].DateTime, assertedCandle.StepDateTime));
    });
    #endregion
  }

  [Fact]
  public async Task WHEN_request_not_exist_instrument_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1);
    var untillDt = new DateTime(2020, 10, 1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(-1, TimeFrame.Enum.D1, TimeFrame.Enum.M, fromDt, untillDt);
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
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1);
    var untillDt = new DateTime(2020, 10, 1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, 99, (int)TimeFrame.Enum.M, fromDt, untillDt);
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
  public async Task WHEN_request_not_exist_chart_timeframe_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.InitInstrumentData();
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1);
    var untillDt = new DateTime(2020, 10, 1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, (int)TimeFrame.Enum.D1, 99, fromDt, untillDt);
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
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1);
    var untillDt = new DateTime(2020, 10, 1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(-1, 99, (int)TimeFrame.Enum.M, fromDt, untillDt);
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
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1);
    var untillDt = new DateTime(2020, 10, 1);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, TimeFrame.Enum.H1, TimeFrame.Enum.M, fromDt, untillDt);
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
    var usedUploadedCandles1 = await this.InitChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.InitChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1);
    var untillDt = new DateTime(2021, 1, 2);
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<ICandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, fromDt, untillDt);
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