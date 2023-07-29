using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

public partial class JoinedChart
{
  protected JoinedChart(DateTime fromDate,
                          DateTime untillDate,
                          int stepChartId,
                          int targetTimeFrameId)
  {
    FromDate = fromDate;
    UntillDate = untillDate;
    StepChartId = stepChartId;
    TargetTimeFrameId = targetTimeFrameId;
  }
  public JoinedChart(DateTime from,
                DateTime untill,
                Chart stepChart,
                TimeFrame targetTimeFrame) : this(from, untill, stepChart.Id, targetTimeFrame.Id)
  {
    StepChart = stepChart;
    TargetTimeFrame = targetTimeFrame;
    Validate();
  }

 
}