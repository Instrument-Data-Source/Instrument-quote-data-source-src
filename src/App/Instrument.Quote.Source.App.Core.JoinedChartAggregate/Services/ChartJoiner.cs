using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.DateTimePeriod;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Instrument.Quote.Source.Shared.Result.Extension;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Service;

public class ChartJoiner : IChartJoiner
{
  private readonly IReadRepository<TimeFrame> timeframeRep;
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<Chart> chartRep;
  private readonly IReadRepository<Candle> candleRep;

  public ChartJoiner(
    IReadRepository<TimeFrame> timeframeRep,
    IReadRepository<ent.Instrument> instrumentRep,
    IReadRepository<Chart> chartRep,
    IReadRepository<Candle> candleRep)
  {
    this.timeframeRep = timeframeRep;
    this.instrumentRep = instrumentRep;
    this.chartRep = chartRep;
    this.candleRep = candleRep;
  }

  public async Task<JoinedChart> JoinToAsync(Chart baseChart, TimeFrame targetTimeFrame, DateTimePeriod joinPeriod, CancellationToken cancellationToken)
  {
    if (targetTimeFrame.EnumId.ToSeconds() <= TimeFrame.GetEnumFrom(baseChart.TimeFrameId).ToSeconds())
      throw new ArgumentException("Target TimeFrame must be GT base Chart TimeFrame", nameof(targetTimeFrame));
    if (joinPeriod.From < baseChart.FromDate || joinPeriod.Untill > baseChart.UntillDate)
    {
      throw new ArgumentOutOfRangeException(nameof(joinPeriod), "Not in range of base chart");
    }
    var targetUntilltDt = targetTimeFrame.EnumId.GetUntillDateTimeFor(joinPeriod.Untill);
    var targetFromDt = targetTimeFrame.EnumId.GetFromDateTimeFor(joinPeriod.From);

    var usedUntillDt = targetUntilltDt < baseChart.UntillDate ? targetUntilltDt : baseChart.UntillDate;
    var usedStartDt = targetFromDt > baseChart.FromDate ? targetFromDt : baseChart.FromDate;
    var usingBaseChart = await BuildUsingChart(baseChart, new DateTimePeriod(usedStartDt, usedUntillDt), cancellationToken);

    JoinedChart newJoinedChart = await CreateJoinedChart(baseChart, targetTimeFrame, usingBaseChart);

    return newJoinedChart;
  }

  private async Task<Chart> BuildUsingChart(Chart baseChart, DateTimePeriod period, CancellationToken cancellationToken)
  {
    TimeFrame timeframe = baseChart.TimeFrame ?? await timeframeRep.GetByIdAsync(baseChart.TimeFrameId, cancellationToken);
    ent.Instrument instrument = baseChart.Instrument ?? await instrumentRep.GetByIdAsync(baseChart.InstrumentId, cancellationToken);
    Candle[] candles = (baseChart.Candles ?? candleRep.Table).Where(c => c.ChartId == baseChart.Id && period.Contain(c.DateTime)).ToArray();

    var usingBaseChart = new Chart(period, instrument, timeframe);
    var result = usingBaseChart.AddCandles(candles);
    if (!result.IsSuccess) result.Throw();
    return usingBaseChart;
  }

  private async Task<JoinedChart> CreateJoinedChart(Chart linkedBaseChart, TimeFrame targetTimeFrame, Chart usingBaseChart)
  {
    if (linkedBaseChart.Instrument == null || linkedBaseChart.TimeFrame == null)
      linkedBaseChart = await chartRep.Table.Include(e => e.Instrument).Include(e => e.TimeFrame).SingleAsync(e => e.Id == linkedBaseChart.Id);
    var newJoinedChart = new JoinedChart(usingBaseChart.FromDate, usingBaseChart.UntillDate, linkedBaseChart, targetTimeFrame);
    IEnumerable<JoinedCandle> joinedCandles = Join(usingBaseChart, newJoinedChart);
    var addResult = newJoinedChart.AddCandles(joinedCandles);
    if (!addResult.IsSuccess) addResult.Throw();
    return newJoinedChart;
  }

  private static IEnumerable<JoinedCandle> Join(
      Chart baseChart,
      JoinedChart newJoinedChart)
  {
    JoinedCandle? prevJoinedCandle = null;
    List<JoinedCandle> joinedCandles = new List<JoinedCandle>();
    bool? isFullCalced = null;
    foreach (var baseCandle in baseChart.Candles!.OrderBy(c => c.DateTime))
    {
      var targetStartDt = ((TimeFrame.Enum)newJoinedChart.TargetTimeFrameId).GetFromDateTimeFor(baseCandle.DateTime);
      if (prevJoinedCandle == null || prevJoinedCandle.TargetDateTime != targetStartDt)
      {
        var targetEndDt = ((TimeFrame.Enum)newJoinedChart.TargetTimeFrameId).GetUntillDateTimeFor(baseCandle.DateTime);
        isFullCalced = baseChart.FromDate <= targetStartDt && targetEndDt <= baseChart.UntillDate;
      }
      JoinedCandle newJoinedCandle;
      if (prevJoinedCandle != null)
        if (prevJoinedCandle.TargetDateTime == targetStartDt)
          newJoinedCandle = new JoinedCandle(
            baseCandle.DateTime, targetStartDt,
            prevJoinedCandle.Open,
            Math.Max(prevJoinedCandle.High, baseCandle.High),
            Math.Min(prevJoinedCandle.Low, baseCandle.Low),
            baseCandle.Close,
            prevJoinedCandle.Volume + baseCandle.Volume,
            false, (bool)isFullCalced!,
             newJoinedChart);
        else
        {
          newJoinedCandle = new JoinedCandle(
            baseCandle.DateTime, targetStartDt,
            baseCandle.Open,
            baseCandle.High, baseCandle.Low,
            baseCandle.Close,
            baseCandle.Volume,
            false, (bool)isFullCalced!,
            newJoinedChart);
          prevJoinedCandle.IsLast = true;
        }
      else
        newJoinedCandle = new JoinedCandle(
          baseCandle.DateTime, targetStartDt,
          baseCandle.Open,
          baseCandle.High, baseCandle.Low,
          baseCandle.Close,
          baseCandle.Volume,
          false, (bool)isFullCalced!,
          newJoinedChart);

      prevJoinedCandle = newJoinedCandle;

      joinedCandles.Add(newJoinedCandle);
    }
    if (prevJoinedCandle != null)
      prevJoinedCandle.IsLast = true;
    return joinedCandles;
  }
}