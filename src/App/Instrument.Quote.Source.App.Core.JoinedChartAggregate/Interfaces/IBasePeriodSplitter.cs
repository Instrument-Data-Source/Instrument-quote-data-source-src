using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.DateTimePeriod;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;

public interface IBasePeriodSplitter
{
  IEnumerable<DateTimePeriod> SplitOnChunks(Chart baseChart, TimeFrame.Enum targetTimeFrame);
  IEnumerable<DateTimePeriod> SplitOnChunks(Chart baseChart, JoinedChart joinedChart);
  IEnumerable<DateTimePeriod> SplitPeriodOnChunks(DateTimePeriod splittedPeriod, TimeFrame.Enum baseTimeFrame, TimeFrame.Enum targetTimeFrame);
}

public static class IBasePeriodSplitterExtension
{

  public static IEnumerable<DateTimePeriod> SplitOnChunks(this IBasePeriodSplitter splitter, Chart baseChart, int targetTimeFrameId)
  {
    return splitter.SplitOnChunks(baseChart, TimeFrame.GetEnumFrom(targetTimeFrameId));
  }
}