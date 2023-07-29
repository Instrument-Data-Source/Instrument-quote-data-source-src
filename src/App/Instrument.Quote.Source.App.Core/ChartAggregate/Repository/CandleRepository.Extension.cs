using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Repository;

public static class CandleRepositoryExtension
{

  public async static Task<IEnumerable<Candle>> GetAsync(this IReadRepository<Candle> candleRep, int chartId, DateTime from, DateTime untill, CancellationToken cancellationToken)
  {
    return await candleRep.Table
                          .Where(e =>
                            e.ChartId == chartId &&
                            e.DateTime >= from &&
                            e.DateTime < untill)
                          .ToArrayAsync(cancellationToken);
  }

  public async static Task<IEnumerable<Candle>> GetAsync(this IReadRepository<Candle> candleRep, Chart chart, CancellationToken cancellationToken)
  {
    return await candleRep.Table
                          .Where(e =>
                            e.ChartId == chart.Id &&
                            e.DateTime >= chart.FromDate &&
                            e.DateTime < chart.UntillDate)
                          .ToArrayAsync(cancellationToken);
  }
}