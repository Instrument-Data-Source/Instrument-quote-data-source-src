using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

public partial class JoinedChart
{
  protected JoinedChart(DateTime fromDate,
                          DateTime untillDate,
                          int baseChartId,
                          int targetTimeFrameId)
  {
    FromDate = fromDate;
    UntillDate = untillDate;
    BaseChartId = baseChartId;
    TargetTimeFrameId = targetTimeFrameId;
  }
  public JoinedChart(DateTime from,
                DateTime untill,
                Chart baseChart,
                TimeFrame targetTimeFrame) : this(from, untill, baseChart.Id, targetTimeFrame.Id)
  {
    BaseChart = baseChart;
    TargetTimeFrame = targetTimeFrame;
    Validate();
  }


}