using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

public partial class JoinedChart
{
  public class Factory : IJoinedChartFactory
  {
    private readonly IReadRepository<Chart> chartRep;
    private readonly IReadRepository<TimeFrame> timeframeRep;
    private readonly IRepository<JoinedChart> joinedChartRep;
    private readonly ITransactionManager transactionManager;
    private readonly ILogger<Factory> logger;

    public Factory(
        IReadRepository<Chart> chartRep,
        IReadRepository<TimeFrame> timeframeRep,
        IRepository<JoinedChart> joinedChartRep,
        ITransactionManager transactionManager,
        ILogger<Factory> logger)
    {
      this.chartRep = chartRep;
      this.timeframeRep = timeframeRep;
      this.joinedChartRep = joinedChartRep;
      this.transactionManager = transactionManager;
      this.logger = logger;
    }
    public async Task<JoinedChart> CreateNewFor(int chartId, int targetTimeFrameId, CancellationToken cancellationToken)
    {
      logger.LogDebug("Get used entities");
      var baseChart = await chartRep.GetByIdAsync(chartId, cancellationToken);
      var targetTimeFrame = await timeframeRep.GetByIdAsync(targetTimeFrameId, cancellationToken);

      var newJoinedChart = new JoinedChart(baseChart.UntillDate, baseChart.UntillDate, baseChart, targetTimeFrame);

      await joinedChartRep.AddAsync(newJoinedChart, cancellationToken: cancellationToken);
     
      await transactionManager.SaveChangesAsync();
      return newJoinedChart;
    }
  }
}