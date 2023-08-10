using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.DateTimePeriod;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;

public interface IChartJoiner
{
  Task<JoinedChart> JoinToAsync(Chart baseChart, TimeFrame targetTimeFrame, DateTimePeriod joinPeriod, CancellationToken cancellationToken);
}

public static class IChartJoinerExtension
{
  public static async Task<JoinedChart> JoinToAsync(this IChartJoiner joiner, Chart baseChart, TimeFrame targetTimeFrame, DateTime from, DateTime untill, CancellationToken cancellationToken = default)
  {
    return await joiner.JoinToAsync(baseChart, targetTimeFrame, new DateTimePeriod(from, untill), cancellationToken);
  }

  public static async Task<JoinedChart> JoinToAsync(this IChartJoiner joiner, Chart baseChart, TimeFrame targetTimeFrame, CancellationToken cancellationToken = default)
  {
    var joinPeriod = new DateTimePeriod(baseChart.FromDate, baseChart.UntillDate);
    return await joiner.JoinToAsync(baseChart, targetTimeFrame, joinPeriod, cancellationToken);
  }
}