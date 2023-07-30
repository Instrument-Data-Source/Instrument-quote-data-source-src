using Ardalis.Result;
using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;

namespace Instrument.Quote.Source.App.Test.JoinedChartAggregate;
public class GetJoinedCandles_Test : BaseDbTest
{

  public GetJoinedCandles_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_new_joined_chart_with_intermediate_candle_THEN_get_correct_data()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 9, 15).ToUniversalTime();
    var expectLastCandleDateTime = new DateTime(2020, 9, 1).ToUniversalTime();
    var expectCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= fromDt && c.DateTime < untillDt).ToArray();
    var expectFirstJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= fromDt && c.DateTime < new DateTime(2020, 3, 1)).ToArray();
    var expectLastJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= new DateTime(2020, 9, 1) && c.DateTime < untillDt).ToArray();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, fromDt, untillDt);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain correct candles count", () => Assert.Equal(expectCandles.Count(), assertedResult.Value.Count()));
    Expect("First candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.ElementAt(0);
      var expectedCandle = expectFirstJoindeCandles.ElementAt(0);
      Expect("Start DT (step) is correct", () => Assert.Equal(expectedCandle.DateTime, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectedCandle.High, assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectedCandle.Low, assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectedCandle.Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectedCandle.Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectedCandle.Volume, assertedCandle.Volume));
      Expect("Is Last is false", () => Assert.False(assertedCandle.IsLast));
      Expect("Target DateTime is Correct", () => Assert.Equal(fromDt, assertedCandle.TargetDateTime));
    });
    Expect("Second candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.ElementAt(1);
      var expectedCandle = new CandleDto()
      {
        DateTime = expectFirstJoindeCandles.ElementAt(1).DateTime,
        High = expectFirstJoindeCandles.Where((dto, idx) => idx < 2).Select(c => c.High).Max(),
        Low = expectFirstJoindeCandles.Where((dto, idx) => idx < 2).Select(c => c.Low).Min(),
        Open = expectFirstJoindeCandles.ElementAt(0).Open,
        Close = expectFirstJoindeCandles.ElementAt(1).Close,
        Volume = expectFirstJoindeCandles.Where((dto, idx) => idx < 2).Select(c => c.Volume).Sum()
      };
      Expect("Start DT (step) is correct", () => Assert.Equal(expectedCandle.DateTime, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectedCandle.High, assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectedCandle.Low, assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectedCandle.Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectedCandle.Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectedCandle.Volume, assertedCandle.Volume));
      Expect("Is Last is false", () => Assert.False(assertedCandle.IsLast));
      Expect("Target DateTime is Correct", () => Assert.Equal(fromDt, assertedCandle.TargetDateTime));
    });
    Expect("Last candle in First group is correct", () =>
    {
      var assertedCandle = assertedResult.Value.Single(e => e.IsLast && e.TargetDateTime == fromDt);
      var expectedCandle = new CandleDto()
      {
        DateTime = expectFirstJoindeCandles.Last().DateTime,
        High = expectFirstJoindeCandles.Select(c => c.High).Max(),
        Low = expectFirstJoindeCandles.Select(c => c.Low).Min(),
        Open = expectFirstJoindeCandles.ElementAt(0).Open,
        Close = expectFirstJoindeCandles.Last().Close,
        Volume = expectFirstJoindeCandles.Select(c => c.Volume).Sum()
      };
      Expect("Start DT (step) is correct", () => Assert.Equal(expectedCandle.DateTime, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectedCandle.High, assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectedCandle.Low, assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectedCandle.Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectedCandle.Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectedCandle.Volume, assertedCandle.Volume));
      Expect("Is Last is True", () => Assert.True(assertedCandle.IsLast));
      Expect("Target DateTime is Correct", () => Assert.Equal(fromDt, assertedCandle.TargetDateTime));
    });

    Expect("Last candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.Last();
      var expectedCandle = new CandleDto()
      {
        DateTime = expectLastJoindeCandles.Last().DateTime,
        High = expectLastJoindeCandles.Select(c => c.High).Max(),
        Low = expectLastJoindeCandles.Select(c => c.Low).Min(),
        Open = expectLastJoindeCandles.ElementAt(0).Open,
        Close = expectLastJoindeCandles.Last().Close,
        Volume = expectLastJoindeCandles.Select(c => c.Volume).Sum()
      };
      Expect("Start DT (step) is correct", () => Assert.Equal(expectedCandle.DateTime, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectedCandle.High, assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectedCandle.Low, assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectedCandle.Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectedCandle.Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectedCandle.Volume, assertedCandle.Volume));
      Expect("Is Last is False", () => Assert.False(assertedCandle.IsLast));
      Expect("Target DateTime is Correct", () => Assert.Equal(expectLastCandleDateTime, assertedCandle.TargetDateTime));
    });
    #endregion
  }

  [Fact]
  public async void WHEN_request_extension_of_joined_chart_with_intermediate_candle_THEN_get_correct_data()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    Logger.LogDebug("1. load candles");
    var step1_period = (new DateTime(2020, 1, 5), new DateTime(2020, 4, 1));
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, step1_period.Item1, step1_period.Item2);

    Logger.LogDebug("2. load joined candles");
    var step2_period = (new DateTime(2020, 1, 10).ToUniversalTime(), new DateTime(2020, 1, 15).ToUniversalTime());
    IEnumerable<JoinedCandleDto> step2_answer;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
      step2_answer = (await usedSrv.GetAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, step2_period.Item1, step2_period.Item2)).Value;
    }

    Logger.LogDebug("3. extend candles");
    var step3_period = (new DateTime(2019, 8, 1).ToUniversalTime(), new DateTime(2020, 1, 5).ToUniversalTime());
    var usedUploadedCandles3 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, step3_period.Item1, step3_period.Item2);

    Logger.LogDebug("Prepare expectation data");
    var act_period = (new DateTime(2019, 12, 31).ToUniversalTime(), new DateTime(2020, 1, 20).ToUniversalTime());
    var expectCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= act_period.Item1 && c.DateTime < act_period.Item2).ToList();
    expectCandles.AddRange(usedUploadedCandles3.Candles.Where(c => c.DateTime >= act_period.Item1 && c.DateTime < act_period.Item2).ToList());

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, act_period.Item1, act_period.Item2);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    var assertedResponse = assertedResult.Value;
    Expect("Result contain correct candles count", () => Assert.Equal(expectCandles.Count(), assertedResponse.Count()));
    Expect("First answer candle was changed", () =>
    {
      //Because we load addition data to month, so month aggregation was changed
      var usedDt = new DateTime(2020, 1, 10).ToUniversalTime();

      // Check that assert data is correct
      Assert.InRange(usedDt, step2_period.Item1, step2_period.Item2);

      var step2_candle = step2_answer.Single(c => c.DateTime == new DateTime(2020, 1, 10).ToUniversalTime());
      var assertedCandle = assertedResponse.Single(c => c.DateTime == new DateTime(2020, 1, 10).ToUniversalTime());
      Expect("Candle has same Date", () => Assert.Equal(step2_candle.DateTime, assertedCandle.DateTime));
      Expect("step2 candle is calced False", () => Assert.False(step2_candle.IsFullCalced));
      Expect("act candle is calced True", () => Assert.True(assertedCandle.IsFullCalced));
      Expect("Volume was change", () => Assert.NotEqual(step2_candle.Volume, assertedCandle.Volume));
      if (step2_candle.Open == assertedCandle.Open &&
         step2_candle.High == assertedCandle.High &&
         step2_candle.Low == assertedCandle.Low &&
         step2_candle.Close == assertedCandle.Close)
        Logger.LogWarning("Step2 candle has same prices as asserted candle. It is barely possible");
    });
    #endregion
  }

  [Fact]
  public async void WHEN_request_joined_chart_THEN_get_correct_data()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 10, 1).ToUniversalTime();
    var expectFirstJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= fromDt && c.DateTime < new DateTime(2020, 3, 1)).ToArray();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
      assertedResult = await usedSrv.GetAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, fromDt, untillDt, true);
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain only last candles (8 candle)", () => Assert.Equal(8, assertedResult.Value.Count()));
    Expect("First candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.ElementAt(0);
      Expect("Start DT is correct", () => Assert.Equal(expectFirstJoindeCandles.Last().DateTime, assertedCandle.DateTime));
      Expect("Higt is correct", () => Assert.Equal(expectFirstJoindeCandles.Select(c => c.High).Max(), assertedCandle.High));
      Expect("Low is correct", () => Assert.Equal(expectFirstJoindeCandles.Select(c => c.Low).Min(), assertedCandle.Low));
      Expect("Open is correct", () => Assert.Equal(expectFirstJoindeCandles[0].Open, assertedCandle.Open));
      Expect("Close is correct", () => Assert.Equal(expectFirstJoindeCandles[expectFirstJoindeCandles.Count() - 1].Close, assertedCandle.Close));
      Expect("Volume is correct", () => Assert.Equal(expectFirstJoindeCandles.Select(c => c.Volume).Sum(), assertedCandle.Volume));
      Expect("IsLast eq true", () => Assert.True(assertedCandle.IsLast));
      Expect("Step DateTime is Correct", () => Assert.Equal(fromDt, assertedCandle.TargetDateTime));
    });

    #endregion
  }
  [Fact]
  public async Task WHEN_request_not_exist_instrument_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 10, 1).ToUniversalTime();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
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

    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 10, 1).ToUniversalTime();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
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

    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 10, 1).ToUniversalTime();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
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

    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 10, 1).ToUniversalTime();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
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

    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 10, 1).ToUniversalTime();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
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

    (var usedInstrument1, var usedInstrument2) = await this.AddMockInstrumentData();
    var usedUploadedCandles1 = await this.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await this.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2021, 1, 2).ToUniversalTime();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<IEnumerable<JoinedCandleDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
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