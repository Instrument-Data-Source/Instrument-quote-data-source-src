using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Config;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.DateTimePeriod;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Service;

public class BasePeriodSplitter : IBasePeriodSplitter
{
  private readonly JoinedChartModuleConfig config;
  private readonly ILogger<BasePeriodSplitter> logger;

  public BasePeriodSplitter(JoinedChartModuleConfig config, ILogger<BasePeriodSplitter> logger)
  {
    this.config = config;
    this.logger = logger;
  }
  public IEnumerable<DateTimePeriod> SplitOnChunks(Chart baseChart, TimeFrame.Enum targetTimeFrame)
  {
    logger.LogInformation("Split all base chart period from {0} untill {1}", baseChart.FromDate, baseChart.UntillDate);
    var periods = SplitPeriodOnChunks(new DateTimePeriod(baseChart.FromDate, baseChart.UntillDate),
                                  TimeFrame.GetEnumFrom(baseChart.TimeFrameId),
                                  targetTimeFrame);
    return periods;
  }

  public IEnumerable<DateTimePeriod> SplitOnChunks(Chart baseChart, JoinedChart joinedChart)
  {
    var _ret_periods = new List<DateTimePeriod>();

    if (baseChart.FromDate < joinedChart.FromDate)
    {
      logger.LogInformation("Split period before joined chart from {0} untill {1}", baseChart.FromDate, joinedChart.FromDate);
      var periods = SplitPeriodOnChunks(new DateTimePeriod(baseChart.FromDate, joinedChart.FromDate),
                                  TimeFrame.GetEnumFrom(baseChart.TimeFrameId),
                                  TimeFrame.GetEnumFrom(joinedChart.TargetTimeFrameId));
      _ret_periods.AddRange(periods.OrderByDescending(dtp => dtp.From));
    }
    if (baseChart.UntillDate > joinedChart.UntillDate)
    {
      logger.LogInformation("Split period after joined chart from {0} untill {1}", joinedChart.UntillDate, baseChart.UntillDate);
      var periods = SplitPeriodOnChunks(new DateTimePeriod(joinedChart.UntillDate, baseChart.UntillDate),
                                 TimeFrame.GetEnumFrom(baseChart.TimeFrameId),
                                 TimeFrame.GetEnumFrom(joinedChart.TargetTimeFrameId));
      _ret_periods.AddRange(periods.OrderBy(dtp => dtp.From));
    }

    logger.LogInformation("Cut period on {0} chunks", _ret_periods.Count());
    return _ret_periods;
  }

  public IEnumerable<DateTimePeriod> SplitPeriodOnChunks(DateTimePeriod splittedPeriod, TimeFrame.Enum baseTimeFrame, TimeFrame.Enum targetTimeFrame)
  {
    var _ret_periods = new List<DateTimePeriod>();
    if (splittedPeriod.IsEmpty())
    {
      logger.LogInformation("Period is empty, no need to split");
      return _ret_periods;
    }

    var period = BuildPeriod(splittedPeriod.From, baseTimeFrame, targetTimeFrame);
    while (period.Untill < splittedPeriod.Untill)
    {
      _ret_periods.Add(period);
      period = BuildPeriod(period.Untill, baseTimeFrame, targetTimeFrame);
    }
    _ret_periods.Add(new DateTimePeriod(period.From, splittedPeriod.Untill));

    logger.LogInformation("Cut period on {0} chunks", _ret_periods.Count());
    return _ret_periods;
  }

  private DateTimePeriod BuildPeriod(DateTime from, TimeFrame.Enum baseTimeFrame, TimeFrame.Enum targetTimeFrame)
  {
    var period_from = from;
    var period_untill_next = targetTimeFrame.GetUntillDateTimeFor(period_from);
    var period_untill = period_untill_next;

    var baseCount = (period_untill_next - period_from).TotalSeconds / baseTimeFrame.ToSeconds();
    while (baseCount < config.MaxSplitBaseCandleCount)
    {
      period_untill = period_untill_next;
      period_untill_next = targetTimeFrame.GetUntillDateTimeFor(period_untill);
      baseCount = (period_untill_next - period_from).TotalSeconds / baseTimeFrame.ToSeconds();
    }
    return new DateTimePeriod(period_from, period_untill);
  }
}