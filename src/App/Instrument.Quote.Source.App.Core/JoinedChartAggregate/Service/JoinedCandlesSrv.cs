using Ardalis.Result;
using FluentValidation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Tools;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Core.ChartAggregate.Repository;
using Instrument.Quote.Source.App.Core.Validation;
using Ardalis.GuardClauses;
using System.Linq.Expressions;
using Instrument.Quote.Source.Shared.Result.Extension;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Service;

public class JoinedCandlesSrv : IReadJoinedCandleSrv
{
  private readonly IReadRepository<Candle> candleRep;
  private readonly IReadRepository<Chart> chartRep;
  private readonly IReadRepository<ent.Instrument> instrumentRep;
  private readonly IReadRepository<TimeFrame> timeframeRep;
  private readonly IRepository<JoinedChart> joinedChartRep;
  private readonly IRepository<JoinedCandle> joinedCandleRep;
  private readonly ITransactionManager transactionManager;
  private readonly ILogger<JoinedCandlesSrv> logger;

  public JoinedCandlesSrv(
      IReadRepository<Candle> candleRep,
      IReadRepository<Chart> chartRep,
      IReadRepository<ent.Instrument> instrumentRep,
      IReadRepository<TimeFrame> timeframeRep,
      IRepository<JoinedChart> joinedChartRep,
      IRepository<JoinedCandle> joinedCandleRep,
      ITransactionManager transactionManager,
      ILogger<JoinedCandlesSrv> logger)
  {
    this.candleRep = candleRep;
    this.chartRep = chartRep;
    this.instrumentRep = instrumentRep;
    this.timeframeRep = timeframeRep;
    this.joinedChartRep = joinedChartRep;
    this.joinedCandleRep = joinedCandleRep;
    this.transactionManager = transactionManager;
    this.logger = logger;
  }

  public async Task<Result<IEnumerable<JoinedCandleDto>>> GetAsync(int instrumentId, int stepTimeFrameId, int targetTimeFrameId,
            [UTCKind] DateTime from, [UTCKind] DateTime untill, bool hideIntermediateCandles = false, CancellationToken cancellationToken = default)
  {
    Guard.Against.AgainstExpression(UTCKindAttribute.IsUTC, from, $"{nameof(from)} must be in UTC kind");
    Guard.Against.AgainstExpression(UTCKindAttribute.IsUTC, untill, $"{nameof(untill)} must be in UTC kind");

    logger.LogDebug("Load exist joined chart");
    var joinedChartResult = await GetExistJoinedChartAsync(instrumentId, stepTimeFrameId, targetTimeFrameId, cancellationToken);
    if (!joinedChartResult.IsSuccess)
      return joinedChartResult.Repack<IEnumerable<JoinedCandleDto>>();

    var joinedChart = joinedChartResult.Value;
    if (joinedChart == null || from < joinedChart.FromDate || untill > joinedChart.UntillDate)
    {
      var baseChart = await chartRep.GetByAsync(instrumentId, stepTimeFrameId, cancellationToken);
      if (from < baseChart.FromDate || untill > baseChart.UntillDate)
        return Result.NotFound(nameof(Candle));

      transactionManager.BeginTransaction();
      try
      {
        if (joinedChart == null)
          joinedChart = await CanculateJoinedChart(baseChart.Id, targetTimeFrameId, cancellationToken);
        else if (from < joinedChart.FromDate || untill > joinedChart.UntillDate)
          await UpdateJoinedChart(from, untill, joinedChart, baseChart.Id, cancellationToken);

        transactionManager.CommitTransaction();
      }
      catch (Exception)
      {
        transactionManager.RollBack();
        throw;
      }

    }

    IEnumerable<JoinedCandle> arr = await SelectJoinedCandles(from, untill, hideIntermediateCandles, joinedChart!);

    var candleMapper = new JoinedCandleMapper(joinedChart!);
    return Result.Success(arr.Select(candleMapper.map));
  }

  private async Task<Result> CheckDoesExistChartFitRequestAsync(int chartId, DateTime from, DateTime untill, CancellationToken cancellationToken)
  {
    var chart = await chartRep.GetByIdAsync(chartId, cancellationToken);
    if (from < chart.FromDate || untill > chart.UntillDate)
      return Result.NotFound(nameof(Candle));
    return Result.Success();
  }

  private async Task<IEnumerable<JoinedCandle>> SelectJoinedCandles(DateTime from, DateTime untill, bool hideIntermediateCandles, JoinedChart joinedChart)
  {
    var query = joinedCandleRep.Table.Where(e => e.JoinedChartId == joinedChart.Id &&
                                                     e.StepDateTime >= from &&
                                                     e.StepDateTime < untill);
    if (hideIntermediateCandles)
      query = query.Where(e => e.IsLast);
    return await query.ToArrayAsync();
  }


  private async Task<JoinedChart?> CanculateJoinedChart(int chartId, int targetTimeFrameId, CancellationToken cancellationToken)
  {
    var baseChart = await chartRep.Table.Include(e => e.Candles)
                                        .Include(e => e.Instrument)
                                        .Include(e => e.TimeFrame)
                                        .GetRep()
                                        .GetByIdAsync(chartId, cancellationToken);
    var targetTimeFrame = await timeframeRep.GetByIdAsync(targetTimeFrameId, cancellationToken);
    var newJoinedChart = baseChart.JoinTo(targetTimeFrame);
    await joinedChartRep.AddAsync(newJoinedChart, cancellationToken: cancellationToken);
    return newJoinedChart;
  }

  private async Task UpdateJoinedChart(DateTime from, DateTime untill, JoinedChart joinedChart, int baseChartId, CancellationToken cancellationToken = default)
  {
    await new JoinedChart.Manager(timeframeRep, chartRep, candleRep, joinedCandleRep).UpdateAsync(joinedChart, cancellationToken);
    await joinedChartRep.SaveChangesAsync();
  }

  public async Task<Result<JoinedChart?>> GetExistJoinedChartAsync(int instrumentId, int stepTimeFrameId, int targetTimeFrameId, CancellationToken cancellationToken)
  {
    var joinedChart = await joinedChartRep.GetByAsync(instrumentId, stepTimeFrameId, targetTimeFrameId, cancellationToken);
    if (joinedChart == null)
    {
      var notFound = new List<string>();
      var chartIsFound = await chartRep.ContainAsync(e => e.TimeFrameId == stepTimeFrameId && e.InstrumentId == instrumentId, cancellationToken);
      if (!chartIsFound)
      {
        if (!await instrumentRep.ContainIdAsync(instrumentId, cancellationToken))
          notFound.Add(nameof(ent.Instrument));
        if (!await timeframeRep.ContainIdAsync(stepTimeFrameId, cancellationToken))
          notFound.Add(nameof(TimeFrame));
        if (notFound.Count() == 0)
          notFound.Add(nameof(Chart));
      }

      if (!await timeframeRep.ContainIdAsync(targetTimeFrameId, cancellationToken))
        notFound.Add(nameof(TimeFrame));

      if (notFound.Count() != 0)
        return Result.NotFound(notFound.Distinct().ToArray());
    }

    return Result.Success(joinedChart);
  }
}