using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Service;

public class ChartSrv : IChartSrv
{
  private readonly IReadRepository<Chart> chartRep;
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly ILogger<ChartSrv> logger;

  public ChartSrv(IReadRepository<Chart> chartRep,
                  IReadRepository<ent.Instrument> instrumentRep,
                  ILogger<ChartSrv> logger)
  {
    this.chartRep = chartRep;
    this.instrumentRep = instrumentRep;
    this.logger = logger;
  }

  public async Task<Result<IEnumerable<ChartDto>>> GetAllLoadedPeriodsAsync(CancellationToken cancellationToken = default)
  {
    IEnumerable<ChartDto> _ret = await chartRep.Table.Select(e => e.ToDto()).ToArrayAsync(cancellationToken);
    return Result.Success(_ret);
  }

  public async Task<Result<IEnumerable<ChartDto>>> GetLoadedPeriodsAsync(int instrumentId, CancellationToken cancellationToken = default)
  {
    IEnumerable<ChartDto> _ret = await chartRep.Table.Where(c => c.InstrumentId == instrumentId).Select(e => e.ToDto()).ToArrayAsync(cancellationToken);

    if (_ret.Count() == 0 && !await instrumentRep.ContainIdAsync(instrumentId, cancellationToken))
      return Result.NotFound(nameof(ent.Instrument));

    return Result.Success(_ret);
  }
}