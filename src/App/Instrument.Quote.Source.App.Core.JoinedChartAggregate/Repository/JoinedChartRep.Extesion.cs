using System.Linq.Expressions;
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

  public static async Task<JoinedChart> GetByAsync(this IReadRepository<JoinedChart> joinedChartRep, int instrumentId, int stepTimeFrameId, int targetTimeFrameId, CancellationToken cancellationToken = default)
  {
    return await joinedChartRep.Table.Include(e => e.StepChart)
                                     .SingleAsync(
                                        GetByFilter(instrumentId, stepTimeFrameId, targetTimeFrameId),
                                        cancellationToken);
  }
  public static async Task<JoinedChart?> TryGetByAsync(this IReadRepository<JoinedChart> joinedChartRep, int instrumentId, int stepTimeFrameId, int targetTimeFrameId, CancellationToken cancellationToken = default)
  {
    return await joinedChartRep.Table.Include(e => e.StepChart)
                                     .SingleOrDefaultAsync(
                                        GetByFilter(instrumentId, stepTimeFrameId, targetTimeFrameId),
                                        cancellationToken);
  }

  private static Expression<Func<JoinedChart, bool>> GetByFilter(int instrumentId,
                                                                  int stepTimeFrameId,
                                                                  int targetTimeFrameId)
  {
    return e => e.StepChart.InstrumentId == instrumentId &&
                e.StepChart.TimeFrameId == stepTimeFrameId &&
                e.TargetTimeFrameId == targetTimeFrameId;
  }
}