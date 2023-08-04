using Ardalis.GuardClauses;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.DateTimePeriod;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Instrument.Quote.Source.Shared.Result.Extension;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

public partial class JoinedChart
{
  public class Manager
  {
    private readonly IReadRepository<Candle> candleRep;
    private readonly IRepository<JoinedCandle> joinedCandleRep;
    private readonly int maxBaseCount;
    private readonly IReadRepository<Chart> chartRep;
    private readonly IReadRepository<TimeFrame> timeframeRep;

    public Manager(IReadRepository<TimeFrame> timeframeRep,
                   IReadRepository<Chart> chartRep,
                   IReadRepository<Candle> candleRep,
                   IRepository<JoinedCandle> joinedCandleRep,
                   int maxBaseCount = 1000)
    {
      this.candleRep = candleRep;
      this.joinedCandleRep = joinedCandleRep;
      this.maxBaseCount = maxBaseCount;
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
      var periods = SplitOnChunks(baseChart, joinedChart, maxBaseCount);
      foreach (var period in periods)
      {
        var fromSideNewJoinedChart = baseChart.JoinTo(targetTimeFrame, period.From, period.Untill, candleRep);
        await Extend(joinedChart, fromSideNewJoinedChart, cancellationToken);
      }
    }

    private async Task Extend(JoinedChart extendedJoinedChart, JoinedChart extensionJoinedChart, CancellationToken cancellationToken = default)
    {
      List<JoinedCandle> updatedCandles;
      if (extendedJoinedChart._joinedCandles == null)
      {
        updatedCandles = await joinedCandleRep.Table.Where(e => e.StepDateTime >= extensionJoinedChart.FromDate &&
                                                                e.StepDateTime < extensionJoinedChart.UntillDate)
                                                          .ToListAsync(cancellationToken);

        //await joinedCandleRep.RemoveRangeAsync(replacedCandles);
      }
      else
      {
        updatedCandles = extendedJoinedChart._joinedCandles.Where(e => e.StepDateTime >= extensionJoinedChart.FromDate && e.StepDateTime < extensionJoinedChart.UntillDate).ToList();
        //replacedCandles.ForEach(c => extendedJoinedChart._joinedCandles.Remove(c));
      }
      var updatedCandlesStepDT = updatedCandles.Select(c => c.StepDateTime).ToArray();

      var addedCandles = extensionJoinedChart.JoinedCandles!.Where(c => !updatedCandlesStepDT.Contains(c.StepDateTime)).ToArray();
      updatedCandles.ForEach(c => c.Update(extensionJoinedChart.JoinedCandles!.Single(nc => nc.StepDateTime == c.StepDateTime)));
      //extendedJoinedChart.AddCandles(addedCandles);
      await joinedCandleRep.AddRangeAsync(addedCandles.Select(c => new JoinedCandle(c.StepDateTime, c.TargetDateTime, c.Open, c.High, c.Low, c.Close, c.Volume, c.IsLast, c.IsFullCalc, extendedJoinedChart)));

      if (extensionJoinedChart.FromDate < extendedJoinedChart.FromDate)
        extendedJoinedChart.FromDate = extensionJoinedChart.FromDate;
      if (extensionJoinedChart.UntillDate > extendedJoinedChart.UntillDate)
        extendedJoinedChart.UntillDate = extensionJoinedChart.UntillDate;
      await joinedCandleRep.SaveChangesAsync();
    }

    public static IEnumerable<DateTimePeriod> SplitOnChunks(Chart baseChart, JoinedChart joinedChart, int maxBaseCandles = 1)
    {
      var _ret_periods = new List<DateTimePeriod>();

      if (baseChart.FromDate < joinedChart.FromDate)
      {
        var periods = SplitPeriodOnChunks(new DateTimePeriod(baseChart.FromDate, joinedChart.FromDate),
                                    TimeFrame.GetEnumFrom(baseChart.TimeFrameId),
                                    TimeFrame.GetEnumFrom(joinedChart.TargetTimeFrameId),
                                    maxBaseCandles);
        _ret_periods.AddRange(periods.OrderByDescending(dtp => dtp.From));
      }
      if (baseChart.UntillDate > joinedChart.UntillDate)
      {
        var periods = SplitPeriodOnChunks(new DateTimePeriod(joinedChart.UntillDate, baseChart.UntillDate),
                                   TimeFrame.GetEnumFrom(baseChart.TimeFrameId),
                                   TimeFrame.GetEnumFrom(joinedChart.TargetTimeFrameId),
                                   maxBaseCandles);
        _ret_periods.AddRange(periods.OrderBy(dtp => dtp.From));
      }

      return _ret_periods;
    }

    public static IEnumerable<DateTimePeriod> SplitPeriodOnChunks(DateTimePeriod splittedPeriod, TimeFrame.Enum baseTimeFrame, TimeFrame.Enum targetTimeFrame, int maxBaseCandles = 1)
    {
      var _ret_periods = new List<DateTimePeriod>();
      if (splittedPeriod.IsEmpty())
        return _ret_periods;

      var period = BuildPeriod(splittedPeriod.From, baseTimeFrame, targetTimeFrame, maxBaseCandles);
      while (period.Untill < splittedPeriod.Untill)
      {
        _ret_periods.Add(period);
        period = BuildPeriod(period.Untill, baseTimeFrame, targetTimeFrame, maxBaseCandles);
      }
      _ret_periods.Add(new DateTimePeriod(period.From, splittedPeriod.Untill));
      return _ret_periods;
    }

    private static DateTimePeriod BuildPeriod(DateTime from, TimeFrame.Enum baseTimeFrame, TimeFrame.Enum targetTimeFrame, int maxBaseCandles = 1)
    {
      var period_from = from;
      var period_untill_next = targetTimeFrame.GetUntillDateTimeFor(period_from);
      var period_untill = period_untill_next;

      var baseCount = (period_untill_next - period_from).TotalSeconds / baseTimeFrame.ToSeconds();
      while (baseCount < maxBaseCandles)
      {
        period_untill = period_untill_next;
        period_untill_next = targetTimeFrame.GetUntillDateTimeFor(period_untill);
        baseCount = (period_untill_next - period_from).TotalSeconds / baseTimeFrame.ToSeconds();
      }
      return new DateTimePeriod(period_from, period_untill);
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
    Guard.Against.Null(baseChart.TimeFrame);
    Guard.Against.Null(baseChart.Instrument);

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
    Guard.Against.Null(baseChart.TimeFrame);
    Guard.Against.Null(baseChart.Instrument);

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
    var addResult = newJoinedChart.AddCandles(joinedCandles);
    if (!addResult.IsSuccess)
      addResult.Throw();
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