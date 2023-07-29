using Ardalis.GuardClauses;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Instrument.Quote.Source.Shared.Result.Extension;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

public partial class JoinedChart
{
  public class Manager
  {
    private readonly IReadRepository<Candle> candleRep;
    private readonly IReadRepository<JoinedChart> joinedChartRep;
    private readonly IRepository<JoinedCandle> joinedCandleRep;
    private readonly IReadRepository<Chart> chartRep;
    private readonly IReadRepository<TimeFrame> timeframeRep;

    public Manager(IReadRepository<TimeFrame> timeframeRep,
                   IReadRepository<Chart> chartRep,
                   IReadRepository<Candle> candleRep,
                   IRepository<JoinedChart> joinedChartRep,
                   IRepository<JoinedCandle> joinedCandleRep)
    {
      this.candleRep = candleRep;
      this.joinedChartRep = joinedChartRep;
      this.joinedCandleRep = joinedCandleRep;
      this.chartRep = chartRep;
      this.timeframeRep = timeframeRep;
    }

    public async Task UpdateAsync(JoinedChart joinedChart, CancellationToken cancellationToken = default)
    {
      var baseChart = await chartRep.Table.Include(e => e.TimeFrame)
                                          .Include(e => e.Instrument)
                                          .GetRep()
                                          .GetByIdAsync(joinedChart.StepChartId, cancellationToken);

      var targetTimeFrame = await timeframeRep.GetByIdAsync(joinedChart.TargetTimeFrameId, cancellationToken);

      if (baseChart.FromDate < joinedChart.FromDate)
      {
        var fromSideNewJoinedChart = baseChart.JoinTo(targetTimeFrame, baseChart.FromDate, joinedChart.FromDate, candleRep);
        await Extend(joinedChart, fromSideNewJoinedChart, cancellationToken);

      }
      if (baseChart.UntillDate > joinedChart.UntillDate)
      {
        var untillSideNewJoinedChart = baseChart.JoinTo(targetTimeFrame, joinedChart.UntillDate, baseChart.UntillDate, candleRep);
        await Extend(joinedChart, untillSideNewJoinedChart, cancellationToken);
      }
    }

    private async Task Extend(JoinedChart extendedJoinedChart, JoinedChart extensionJoinedChart, CancellationToken cancellationToken = default)
    {
      // TODO Define validation 
      if (extendedJoinedChart._joinedCandles == null)
      {
        var replacedCandles = await joinedCandleRep.Table.Where(e => e.StepDateTime >= extensionJoinedChart.FromDate &&
                                                                     e.StepDateTime < extensionJoinedChart.UntillDate)
                                                          .ToListAsync(cancellationToken);
        await joinedCandleRep.RemoveRangeAsync(replacedCandles);
      }
      else
      {
        var replacedCandles = extendedJoinedChart._joinedCandles.Where(e => e.StepDateTime >= extensionJoinedChart.FromDate && e.StepDateTime < extensionJoinedChart.UntillDate).ToList();
        replacedCandles.ForEach(c => extendedJoinedChart._joinedCandles.Remove(c));
      }
      extendedJoinedChart.AddCandles(extensionJoinedChart.JoinedCandles!);

      if (extensionJoinedChart.FromDate < extendedJoinedChart.FromDate)
        extendedJoinedChart.FromDate = extensionJoinedChart.FromDate;
      if (extensionJoinedChart.UntillDate > extendedJoinedChart.UntillDate)
        extendedJoinedChart.UntillDate = extensionJoinedChart.UntillDate;
    }
  }
}

public static class JoinedChartManagerFunction
{
  /// <summary>
  /// Join chart to specific timeframe
  /// </summary>
  /// <param name="baseChart"></param>
  /// <param name="targetTimeFrame"></param>
  /// <returns></returns>
  public static JoinedChart JoinTo(
      this Chart baseChart,
      TimeFrame targetTimeFrame)
  {
    if (targetTimeFrame.EnumId.ToSeconds() <= TimeFrame.GetEnumFrom(baseChart.TimeFrameId).ToSeconds())
      throw new ArgumentException("Target TimeFrame must be GT base Chart TimeFrame", nameof(targetTimeFrame));

    JoinedChart newJoinedChart = CreateJoinedChart(baseChart, targetTimeFrame, baseChart);
    return newJoinedChart;
  }

  /// <summary>
  /// Join part of base chart to specific timeframe
  /// </summary>
  /// <param name="baseChart"></param>
  /// <param name="targetTimeFrame"></param>
  /// <param name="from"></param>
  /// <param name="untill"></param>
  /// <param name="candleRep"></param>
  /// <returns></returns>
  public static JoinedChart JoinTo(
      this Chart baseChart,
      TimeFrame targetTimeFrame,
      DateTime from,
      DateTime untill,
      IReadRepository<Candle> candleRep)
  {
    if (targetTimeFrame.EnumId.ToSeconds() <= TimeFrame.GetEnumFrom(baseChart.TimeFrameId).ToSeconds())
      throw new ArgumentException("Target TimeFrame must be GT base Chart TimeFrame", nameof(targetTimeFrame));
    Guard.Against.OutOfRange(from, nameof(from), baseChart.FromDate, baseChart.UntillDate, "Out of exist chart data");
    Guard.Against.OutOfRange(untill, nameof(untill), baseChart.FromDate, baseChart.UntillDate, "Out of exist chart data");

    var targetUntilltDt = targetTimeFrame.EnumId.GetUntillDateTimeFor(untill);
    var targetFromDt = targetTimeFrame.EnumId.GetFromDateTimeFor(from);

    var candles = (baseChart.Candles ?? candleRep.Table).Where(c => c.DateTime >= targetFromDt && c.DateTime < targetUntilltDt).ToArray();

    // If Target TF untill datetime is more that baseChart.UntillDate. It mean that we haven't got enought data for full calc of last candle
    // Get min of both dates
    var usedUntillDt = targetUntilltDt < baseChart.UntillDate ? targetUntilltDt : baseChart.UntillDate;
    var usedStartDt = targetFromDt > baseChart.FromDate ? targetFromDt : baseChart.FromDate;
    var usingBaseChart = new Chart(usedStartDt, usedUntillDt, baseChart.Instrument, baseChart.TimeFrame);

    var result = usingBaseChart.AddCandles(candles);
    if (!result.IsSuccess) result.Throw();

    JoinedChart newJoinedChart = CreateJoinedChart(baseChart, targetTimeFrame, usingBaseChart);

    return newJoinedChart;
  }

  private static JoinedChart CreateJoinedChart(Chart linkedBaseChart, TimeFrame targetTimeFrame, Chart usingBaseChart)
  {
    var newJoinedChart = new JoinedChart(usingBaseChart.FromDate, usingBaseChart.UntillDate, linkedBaseChart, targetTimeFrame);
    IEnumerable<JoinedCandle> joinedCandles = Join(usingBaseChart, newJoinedChart);
    newJoinedChart.AddCandles(joinedCandles);
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