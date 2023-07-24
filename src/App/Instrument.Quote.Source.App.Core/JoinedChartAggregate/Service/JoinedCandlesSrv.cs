using Ardalis.Result;
using FluentValidation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.ChartAggregate.Service;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Service;

public class JoinedCandlesSrv : baseCandleSrv, IReadJoinedCandleSrv
{
  private readonly IRepository<JoinedChart> joinedChartRep;
  private readonly IReadRepository<JoinedCandle> joinedCandleRep;

  public JoinedCandlesSrv(
      IRepository<Chart> chartRep,
      IReadRepository<ent.Instrument> instrumentRep,
      IReadRepository<TimeFrame> timeframeRep,
      IRepository<JoinedChart> joinedChartRep,
      IReadRepository<JoinedCandle> joinedCandleRep,
      ILogger<JoinedCandlesSrv> logger) : base(chartRep, instrumentRep, timeframeRep, logger)
  {
    this.joinedChartRep = joinedChartRep;
    this.joinedCandleRep = joinedCandleRep;
  }

  public async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(int instrumentId, int baseTimeFrameId, int chartTimeFrameId, DateTime from, DateTime untill, bool addIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    logger.LogDebug("Load exist chart");
    var chartResult = await GetExistChartAsync(instrumentId, baseTimeFrameId, cancellationToken);
    if (!chartResult.IsSuccess)
      return chartResult.Repack<IEnumerable<JoinedCandleDto>>();
    var chart = chartResult.Value;

    if (from < chart.FromDate || untill > chart.UntillDate)
      return Result.NotFound(nameof(Candle));

    var targetTf = await timeframeRep.TryGetByIdAsync(chartTimeFrameId, cancellationToken);
    if (targetTf == null)
      return Result.NotFound(nameof(TimeFrame));

    //IEnumerable<Candle> arr = await joinedCandleRep.Table
    //                                             .Where(e =>
    //                                               e.ChartId == chart.Id &&
    //                                               e.DateTime >= from &&
    //                                               e.DateTime < untill)
    //                                             .ToArrayAsync();
    throw new NotImplementedException();
  }
}