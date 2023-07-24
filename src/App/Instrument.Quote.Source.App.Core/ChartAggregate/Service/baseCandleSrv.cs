using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Service;

public abstract class baseCandleSrv
{
  protected readonly IRepository<Chart> chartRep;
  protected readonly IReadRepository<ent.Instrument> instrumentRep;
  protected readonly IReadRepository<TimeFrame> timeframeRep;
  protected readonly ILogger logger;

  public baseCandleSrv(
      IRepository<Chart> chartRep,
      IReadRepository<ent.Instrument> instrumentRep,
      IReadRepository<TimeFrame> timeframeRep,
      ILogger logger)
  {
    this.chartRep = chartRep;
    this.instrumentRep = instrumentRep;
    this.timeframeRep = timeframeRep;
    this.logger = logger;
  }

  public async Task<Result<(ent.Instrument, TimeFrame)>> getEntityAsync(int instrumentId, int timeFrameId, CancellationToken cancellationToken)
  {
    var instrument = await instrumentRep.TryGetByIdAsync(instrumentId, cancellationToken);
    var timeframe = await timeframeRep.TryGetByIdAsync(timeFrameId, cancellationToken);
    var notFound = new List<string>();

    if (instrument == null)
      notFound.Add(nameof(ent.Instrument));
    if (timeframe == null)
      notFound.Add(nameof(TimeFrame));

    if (notFound.Count != 0)
      return Result.NotFound(notFound.ToArray());

    return Result.Success((instrument!, timeframe!));
  }

  public async Task<Result<Chart>> GetExistChartAsync(int instrumentId, int timeFrameId, CancellationToken cancellationToken = default)
  {
    var chart = await chartRep.Table.Include(c => c.Instrument).SingleOrDefaultAsync(e => e.TimeFrameId == timeFrameId && e.InstrumentId == instrumentId, cancellationToken);
    if (chart == null)
    {
      var notFound = new List<string>();
      if (!await instrumentRep.ContainIdAsync(instrumentId, cancellationToken))
        notFound.Add(nameof(ent.Instrument));
      if (!await timeframeRep.ContainIdAsync(timeFrameId, cancellationToken))
        notFound.Add(nameof(TimeFrame));
      if (notFound.Count() == 0)
        notFound.Add(nameof(Chart));
      return Result.NotFound(notFound.ToArray());
    }

    return Result.Success(chart);
  }
}
