using System.Diagnostics.CodeAnalysis;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;
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
                int targetTimeFrameId,
                int chartId)
  {
    StepDateTime = stepDateTime;
    TargetDateTime = targetDateTime;
    Open = open;
    High = high;
    Low = low;
    Close = close;
    Volume = volume;
    IsLast = isLast;
    TargetTimeFrameId = targetTimeFrameId;
    ChartId = chartId;
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
                TimeFrame targetTimeFrame,
                [NotNull] Chart chart) : this(stepDateTime, targetDateTime, open, high, low, close, volume, isLast, targetTimeFrame.Id, chart.Id)
  {
    TargetTimeFrame = targetTimeFrame;
    Chart = chart;
    Validate();
  }
}