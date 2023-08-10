using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.DateTimePeriod;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

public partial class JoinedChart
{
  public class Manager : IJoinedChartManager
  {
    private readonly IRepository<JoinedCandle> joinedCandleRep;
    private readonly IReadRepository<JoinedChart> joinedChartRep;
    private readonly IBasePeriodSplitter basePeriodSplitter;
    private readonly IChartJoiner chartJoiner;
    private readonly ITransactionManager transactionManager;
    private readonly ILogger<Manager> logger;

    public Manager(
        IRepository<JoinedCandle> joinedCandleRep,
        IReadRepository<JoinedChart> joinedChartRep,
        IBasePeriodSplitter basePeriodSplitter,
        IChartJoiner chartJoiner,
        ITransactionManager transactionManager,
        ILogger<Manager> logger)
    {
      this.joinedCandleRep = joinedCandleRep;
      this.joinedChartRep = joinedChartRep;
      this.basePeriodSplitter = basePeriodSplitter;
      this.chartJoiner = chartJoiner;
      this.transactionManager = transactionManager;
      this.logger = logger;
    }

    public async Task UpdateAsync(int joinedChartId, CancellationToken cancellationToken = default)
    {
      logger.LogDebug("Get used entities");
      var joinedChart = await joinedChartRep.Table
                                      .Include(e => e.StepChart).Include(e => e.TargetTimeFrame)
                                      .GetRep()
                                      .GetByIdAsync(joinedChartId, cancellationToken);

      logger.LogDebug("Split unjoined period of base chart on chunks");
      var splittedPeriods = basePeriodSplitter.SplitOnChunks(joinedChart.StepChart, joinedChart);

      logger.LogInformation("Calculate Joined Chart for new periods");
      await ProcessPeriods(joinedChart, splittedPeriods, cancellationToken);
    }

    private async Task<JoinedChart> ProcessPeriods(JoinedChart joinedChart,
                                                  IEnumerable<DateTimePeriod> splittedPeriods,
                                                  CancellationToken cancellationToken)
    {
      foreach (var period in splittedPeriods)
      {
        logger.LogInformation("Join {0} from {1} untill {2}", joinedChart.StepChart.Id, period.From, period.Untill);
        var newJoinedChart = await chartJoiner.JoinToAsync(joinedChart.StepChart, joinedChart.TargetTimeFrame, period, cancellationToken);
        logger.LogInformation("Extend joined chart {0} by new period from {1} untill {2}", joinedChart.Id, period.From, period.Untill);
        await Extend(joinedChart, newJoinedChart, cancellationToken);
      }

      return joinedChart!;
    }

    private async Task Extend(JoinedChart extendedJoinedChart, JoinedChart extensionJoinedChart, CancellationToken cancellationToken = default)
    {
      logger.LogDebug("Get exist candles, which would changed");
      var updatedCandles = await (extendedJoinedChart.JoinedCandles?.AsQueryable() ?? joinedCandleRep.Table)
                                                    .Where(e => e.JoinedChartId == extendedJoinedChart.Id &&
                                                                e.StepDateTime >= extensionJoinedChart.FromDate &&
                                                                e.StepDateTime < extensionJoinedChart.UntillDate)
                                                          .ToListAsync(cancellationToken);
      var updatedCandlesStepDT = updatedCandles.Select(c => c.StepDateTime).ToHashSet();

      logger.LogDebug("Get list of new candles");
      var addedCandles = extensionJoinedChart.JoinedCandles!.Where(c => !updatedCandlesStepDT.Contains(c.StepDateTime)).ToArray();

      logger.LogInformation("Update {0} exist joined candles", updatedCandles.Count);
      updatedCandles.ForEach(c => c.Update(extensionJoinedChart.JoinedCandles!.Single(nc => nc.StepDateTime == c.StepDateTime)));

      logger.LogInformation("Add {0} new joined candles", addedCandles.Length);
      await joinedCandleRep.AddRangeAsync(addedCandles.Select(c => new JoinedCandle(c.StepDateTime, c.TargetDateTime, c.Open, c.High, c.Low, c.Close, c.Volume, c.IsLast, c.IsFullCalc, extendedJoinedChart)));

      logger.LogInformation("Update joined chart period");
      if (extensionJoinedChart.FromDate < extendedJoinedChart.FromDate)
        extendedJoinedChart.FromDate = extensionJoinedChart.FromDate;
      if (extensionJoinedChart.UntillDate > extendedJoinedChart.UntillDate)
        extendedJoinedChart.UntillDate = extensionJoinedChart.UntillDate;

      await transactionManager.SaveChangesAsync(cancellationToken);
    }
  }
}