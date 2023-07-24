using System.Diagnostics.CodeAnalysis;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
public partial class JoinedCandle
{
  protected JoinedCandle(
                DateTime stepDateTime,
                DateTime targetDateTime,
                int open,
                int high,
                int low,
                int close,
                int volume,
                bool isLast,
                int joinedChartId)
  {
    StepDateTime = stepDateTime;
    TargetDateTime = targetDateTime;
    Open = open;
    High = high;
    Low = low;
    Close = close;
    Volume = volume;
    IsLast = isLast;
    JoinedChartId = joinedChartId;
  }

  public JoinedCandle(
                DateTime stepDateTime,
                DateTime targetDateTime,
                int open,
                int high,
                int low,
                int close,
                int volume,
                bool isLast,
                [NotNull] JoinedChart joinedChart) : this(stepDateTime, targetDateTime, open, high, low, close, volume, isLast, joinedChart.Id)
  {
    JoinedChart = joinedChart;
    Validate();
  }
}