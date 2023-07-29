using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Repository;
public static class ChartRepository
{

  public static async Task<Chart> GetByAsync(this IReadRepository<Chart> loadedPeriodRep, int instrumentId, int timeFrameId, CancellationToken cancellationToken = default)
  {
    var loadedPer = await loadedPeriodRep.TryGetForAsync(instrumentId, timeFrameId, cancellationToken);
    if (loadedPer == null)
      throw new ArgumentException("No data for InstrumentId and TimeFrameId");
    return loadedPer;
  }

  public static async Task<Chart?> TryGetForAsync(this IReadRepository<Chart> loadedPeriodRep, int instrumentId, int timeFrameId, CancellationToken cancellationToken = default)
  {
    return await loadedPeriodRep.Table.SingleOrDefaultAsync(e => e.TimeFrameId == timeFrameId && e.InstrumentId == instrumentId, cancellationToken);
  }

}