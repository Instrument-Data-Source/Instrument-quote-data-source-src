using System.Diagnostics.CodeAnalysis;

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
                bool isFullCalc,
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
    IsFullCalc = isFullCalc;
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
                bool isFullCalc,
                [NotNull] JoinedChart joinedChart) : this(stepDateTime, targetDateTime, open, high, low, close, volume, isLast, isFullCalc, joinedChart.Id)
  {
    JoinedChart = joinedChart;
    Validate();
  }


}
