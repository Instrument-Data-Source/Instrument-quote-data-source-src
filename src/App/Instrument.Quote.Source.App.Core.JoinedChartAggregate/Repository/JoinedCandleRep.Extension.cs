using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Service;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Repository;

public static class JoinedCandleRepExtension
{
  public static async Task<JoinedCandleDto[]> GetCandlesAsDtoAsync(this IReadRepository<JoinedCandle> joinedCandleRep, DateTime from, DateTime untill,
                                                                          bool hideIntermediateCandles,
                                                                        JoinedChart joinedChart, CancellationToken cancellationToken = default)
  {

    var query = joinedCandleRep.Table.Include(e => e.JoinedChart)
                                      .ThenInclude(e => e.StepChart)
                                      .ThenInclude(e => e.Instrument)
                                      .Where(e => e.JoinedChartId == joinedChart.Id &&
                                                  e.StepDateTime >= from &&
                                                  e.StepDateTime < untill);
    if (hideIntermediateCandles)
      query = query.Where(e => e.IsLast);

    var ret = await query.ToArrayAsync();
    if (ret.Length == 0)
      return new JoinedCandleDto[0];

    var mapper = new JoinedCandleMapper(ret[0].JoinedChart);

    return ret.Select(mapper.map).ToArray();

  }

}