using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.JoinedChartAggregate;

public class RequestCandles_Test : BaseDbTest
{

  public RequestCandles_Test(ITestOutputHelper output) : base(output)
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


    #region Act 1
    Logger.LogDebug("Test ACT 1: make first request of new data");

    Result<JoinedCandleResponse> assertedResult_act1;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
      assertedResult_act1 = await usedSrv.RequestAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, fromDt, untillDt);
    }

    #endregion


    #region Assert 1
    Logger.LogDebug("Test ASSERT 1");

    Expect("Result is success", () => Assert.True(assertedResult_act1.IsSuccess));
    Expect("Resust has status is InProgress", () => Assert.Equal(JoinedCandleResponse.EnumStatus.InProgress, assertedResult_act1.Value.Status));

    #endregion

    #region Act 2
    Logger.LogDebug("Test ACT 2: Make second request for new data");

    Result<JoinedCandleResponse>? assertedResult = null;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedSrv = sp.GetRequiredService<IJoinedCandleSrv>();
      for (int i = 0; i < 10; i++)
      {
        assertedResult = await usedSrv.RequestAsync(usedInstrument1.Id, TimeFrame.Enum.D1, TimeFrame.Enum.M, fromDt, untillDt);
        Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
        if (assertedResult.Value.Status != JoinedCandleResponse.EnumStatus.Ready)
        {
          Logger.LogDebug($"Get result {assertedResult.Value.Status}");
          assertedResult = null;
          Thread.Sleep(500);
        }
        else
          break;
      }
    }
    #endregion

    #region Assert 2
    Logger.LogDebug("Test ASSERT 2");
    Expect("Result not null", () => Assert.NotNull(assertedResult));

    Expect("Result is success", () => Assert.True(assertedResult!.IsSuccess));
    Expect("Resust has status is InProgress", () => Assert.Equal(JoinedCandleResponse.EnumStatus.Ready, assertedResult_act1.Value.Status));

    Expect("Result contain correct candles count", () => Assert.Equal(expectCandles.Count(), assertedResult!.Value.JoinedCandles.Count()));
    Expect("First candle is correct", () =>
    {
      var assertedCandle = assertedResult!.Value.JoinedCandles.ElementAt(0);
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
      var assertedCandle = assertedResult!.Value.JoinedCandles.ElementAt(1);
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
      var assertedCandle = assertedResult!.Value.JoinedCandles.Single(e => e.IsLast && e.TargetDateTime == fromDt);
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
      var assertedCandle = assertedResult!.Value.JoinedCandles.Last();
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
}