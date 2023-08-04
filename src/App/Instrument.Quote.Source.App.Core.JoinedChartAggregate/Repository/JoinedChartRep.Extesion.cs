using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Repository;

public static class JoinedChartRepExtension
{
  public static async Task<JoinedChart?> GetByAsync(this IReadRepository<JoinedChart> joinedChartRep, int chartId, int targetTimeFrameId, CancellationToken cancellationToken = default)
  {

    return await joinedChartRep.Table.Include(e => e.StepChart).SingleOrDefaultAsync(c => c.StepChartId == chartId && c.TargetTimeFrameId == targetTimeFrameId, cancellationToken);
  }

  public static async Task<JoinedChart?> GetByAsync(this IReadRepository<JoinedChart> joinedChartRep, int instrumentId, int stepTimeFrameId, int targetTimeFrameId, CancellationToken cancellationToken = default)
  {
    return await joinedChartRep.Table.Include(e => e.StepChart)
                                    .SingleOrDefaultAsync(e =>
                                      e.StepChart.InstrumentId == instrumentId &&
                                      e.StepChart.TimeFrameId == stepTimeFrameId &&
                                      e.TargetTimeFrameId == targetTimeFrameId,
                                      cancellationToken);
  }
}