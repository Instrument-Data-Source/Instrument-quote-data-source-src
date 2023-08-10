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
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using MediatR;

namespace Instrument.Quote.Source.App.Test.JoinedChartAggregate;


public class GetJoinedCandles_Test : BaseTest
{
  public GetJoinedCandles_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_new_joined_chart_with_intermediate_candle_THEN_get_correct_data()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await hostFixture.Services.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 9, 15).ToUniversalTime();
    var expectLastCandleDateTime = new DateTime(2020, 9, 1).ToUniversalTime();
    var expectCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= fromDt && c.DateTime < untillDt).ToArray();
    var expectFirstJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= fromDt && c.DateTime < new DateTime(2020, 3, 1)).ToArray();
    var expectLastJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= new DateTime(2020, 9, 1) && c.DateTime < untillDt).ToArray();

    var usedRequest = new GetJoinedChartRequestDto()
    {
      instrumentId = usedInstrument1.Id,
      stepTimeFrameId = ((int)TimeFrame.Enum.D1),
      targetTimeFrameId = ((int)TimeFrame.Enum.M),
      from = fromDt,
      untill = untillDt,
      hideIntermediateCandles = false
    };
    #endregion

    #region Act 1
    Logger.LogDebug("Test ACT 1");

    Result<GetJoinedCandleResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      usedRequest.Validate(sp);
      var mediator = sp.GetRequiredService<IMediator>();
      assertedResult = await mediator.Send(usedRequest);
    }

    #endregion

    #region Assert 1
    Logger.LogDebug("Test ASSERT 1");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain correct candles count", () => Assert.Equal(0, assertedResult.Value.JoinedCandles.Count()));
    Expect("Result contain status is inProcess", () => Assert.Equal(GetJoinedCandleResponseDto.EnumStatus.InProgress, assertedResult.Value.Status));

    #endregion

    #region Act 2
    Logger.LogDebug("Test ACT 2");
    assertedResult = await hostFixture.WaitUntillJoinedDataCalculated(usedRequest);
    #endregion


    #region Assert 2
    Logger.LogDebug("Test ASSERT 2");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain correct candles count", () => Assert.Equal(expectCandles.Count(), assertedResult.Value.JoinedCandles.Count()));
    Expect("Result contain status is inProcess", () => Assert.Equal(GetJoinedCandleResponseDto.EnumStatus.Ready, assertedResult.Value.Status));
    Expect("First candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.JoinedCandles.ElementAt(0);
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
      var assertedCandle = assertedResult.Value.JoinedCandles.ElementAt(1);
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
      var assertedCandle = assertedResult.Value.JoinedCandles.Single(e => e.IsLast && e.TargetDateTime == fromDt);
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
      var assertedCandle = assertedResult.Value.JoinedCandles.Last();
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
    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    Logger.LogDebug("1. load candles");
    var step1_period = (new DateTime(2020, 1, 5), new DateTime(2020, 4, 1));
    var usedUploadedCandles1 = await hostFixture.Services.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, step1_period.Item1, step1_period.Item2);

    Logger.LogDebug("2. load joined candles");
    var step2_period = (new DateTime(2020, 1, 10).ToUniversalTime(), new DateTime(2020, 1, 15).ToUniversalTime());
    var step2_usedRequest = new GetJoinedChartRequestDto()
    {
      instrumentId = usedInstrument1.Id,
      stepTimeFrameId = ((int)TimeFrame.Enum.D1),
      targetTimeFrameId = ((int)TimeFrame.Enum.M),
      from = step2_period.Item1,
      untill = step2_period.Item2,
      hideIntermediateCandles = false
    };
    GetJoinedCandleResponseDto step2_answer;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      step2_usedRequest.Validate(sp);
      var mediator = sp.GetRequiredService<IMediator>();
      step2_answer = (await mediator.Send(step2_usedRequest)).Value;
    }
    var step2_result = await hostFixture.WaitUntillJoinedDataCalculated(step2_usedRequest);

    Logger.LogDebug("3. extend candles");
    var step3_period = (new DateTime(2019, 8, 1).ToUniversalTime(), new DateTime(2020, 1, 5).ToUniversalTime());
    var usedUploadedCandles3 = await hostFixture.Services.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, step3_period.Item1, step3_period.Item2);

    Logger.LogDebug("Prepare expectation request");
    var act_period = (new DateTime(2019, 12, 31).ToUniversalTime(), new DateTime(2020, 1, 20).ToUniversalTime());
    var expectCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= act_period.Item1 && c.DateTime < act_period.Item2).ToList();
    expectCandles.AddRange(usedUploadedCandles3.Candles.Where(c => c.DateTime >= act_period.Item1 && c.DateTime < act_period.Item2).ToList());

    var usedRequest = new GetJoinedChartRequestDto()
    {
      instrumentId = usedInstrument1.Id,
      stepTimeFrameId = ((int)TimeFrame.Enum.D1),
      targetTimeFrameId = ((int)TimeFrame.Enum.M),
      from = act_period.Item1,
      untill = act_period.Item2,
      hideIntermediateCandles = false
    };
    #endregion


    #region Act 1
    Logger.LogDebug("Test ACT 1 Try get New Data");

    Result<GetJoinedCandleResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      usedRequest.Validate(sp);
      var mediator = sp.GetRequiredService<IMediator>();
      assertedResult = await mediator.Send(usedRequest);
    }

    #endregion

    #region Assert 1 
    Logger.LogDebug("Test ASSERT 1");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain correct candles count", () => Assert.True(assertedResult.Value.JoinedCandles.Count() < expectCandles.Count()));
    Expect("Result contain status is inProcess", () => Assert.Equal(GetJoinedCandleResponseDto.EnumStatus.PartlyReady, assertedResult.Value.Status));

    #endregion

    #region Act 2
    Logger.LogDebug("Test ACT 2 Try get New Data after calculation");
    assertedResult = await hostFixture.WaitUntillJoinedDataCalculated(usedRequest);

    #endregion

    #region Assert 2
    Logger.LogDebug("Test ASSERT 2");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    var assertedResponse = assertedResult.Value;
    Expect("Result contain correct candles count", () => Assert.Equal(expectCandles.Count(), assertedResponse.JoinedCandles.Count()));
    Expect("First answer candle was changed", () =>
    {
      //Because we load addition data to month, so month aggregation was changed
      var usedDt = new DateTime(2020, 1, 10).ToUniversalTime();

      // Check that assert data is correct
      Assert.InRange(usedDt, step2_period.Item1, step2_period.Item2);
      var step2_candle = step2_result.Value.JoinedCandles.Single(c => c.DateTime == new DateTime(2020, 1, 10).ToUniversalTime());
      var assertedCandle = assertedResponse.JoinedCandles.Single(c => c.DateTime == new DateTime(2020, 1, 10).ToUniversalTime());
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
  public async void WHEN_request_joined_chart_without_intermediate_candles_THEN_get_correct_data()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await hostFixture.Services.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 10, 1).ToUniversalTime();
    var expectFirstJoindeCandles = usedUploadedCandles1.Candles.Where(c => c.DateTime >= fromDt && c.DateTime < new DateTime(2020, 3, 1)).ToArray();

    var usedRequest = new GetJoinedChartRequestDto()
    {
      instrumentId = usedInstrument1.Id,
      stepTimeFrameId = ((int)TimeFrame.Enum.D1),
      targetTimeFrameId = ((int)TimeFrame.Enum.M),
      from = fromDt,
      untill = untillDt,
      hideIntermediateCandles = true
    };
    #endregion


    #region Act 1
    Logger.LogDebug("Test ACT 1");

    Result<GetJoinedCandleResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      usedRequest.Validate(sp);
      var mediator = sp.GetRequiredService<IMediator>();
      assertedResult = await mediator.Send(usedRequest);
    }

    #endregion

    #region Assert 1 
    Logger.LogDebug("Test ASSERT 1");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain correct candles count", () => Assert.Equal(0, assertedResult.Value.JoinedCandles.Count()));
    Expect("Result contain status is inProcess", () => Assert.Equal(GetJoinedCandleResponseDto.EnumStatus.InProgress, assertedResult.Value.Status));

    #endregion

    #region Act 2
    Logger.LogDebug("Test ACT 2 Try get New Data after calculation");
    assertedResult = await hostFixture.WaitUntillJoinedDataCalculated(usedRequest);
    #endregion

    #region Assert 2
    Logger.LogDebug("Test ASSERT 2");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain only last candles (8 candle)", () => Assert.Equal(8, assertedResult.Value.JoinedCandles.Count()));
    Expect("First candle is correct", () =>
    {
      var assertedCandle = assertedResult.Value.JoinedCandles.ElementAt(0);
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
  public async Task WHEN_request_not_exist_chart_THEN_get_nofFound()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");

    (var usedInstrument1, var usedInstrument2) = await hostFixture.Services.AddMockInstrumentData();
    var usedUploadedCandles1 = await hostFixture.Services.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await hostFixture.Services.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2020, 10, 1).ToUniversalTime();

    var usedRequest = new GetJoinedChartRequestDto()
    {
      instrumentId = usedInstrument1.Id,
      stepTimeFrameId = ((int)TimeFrame.Enum.H1),
      targetTimeFrameId = ((int)TimeFrame.Enum.M),
      from = fromDt,
      untill = untillDt,
      hideIntermediateCandles = false
    };
    using (var array_scope = hostFixture.Services.CreateScope())
    {
      var sp = array_scope.ServiceProvider;
      usedRequest.Validate(sp);
    }
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<GetJoinedCandleResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var mediator = sp.GetRequiredService<IMediator>();
      assertedResult = await mediator.Send(usedRequest);
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
    var usedUploadedCandles1 = await hostFixture.Services.AddMockChartData(usedInstrument1, TimeFrame.Enum.D1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var usedUploadedCandles2 = await hostFixture.Services.AddMockChartData(usedInstrument2, TimeFrame.Enum.H1, new DateTime(2020, 1, 1), new DateTime(2021, 1, 1));
    var fromDt = new DateTime(2020, 2, 1).ToUniversalTime();
    var untillDt = new DateTime(2021, 1, 2).ToUniversalTime();

    var usedRequest = new GetJoinedChartRequestDto()
    {
      instrumentId = usedInstrument1.Id,
      stepTimeFrameId = ((int)TimeFrame.Enum.D1),
      targetTimeFrameId = ((int)TimeFrame.Enum.M),
      from = fromDt,
      untill = untillDt,
      hideIntermediateCandles = false
    };
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    Result<GetJoinedCandleResponseDto> assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      usedRequest.Validate(sp);
      var mediator = sp.GetRequiredService<IMediator>();
      assertedResult = await mediator.Send(usedRequest);
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