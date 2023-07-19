using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
public interface IChartSrv
{
  Task<Result<IEnumerable<ChartDto>>> GetLoadedPeriodsAsync(int instrumentId, CancellationToken cancellationToken = default);
  Task<Result<IEnumerable<ChartDto>>> GetAllLoadedPeriodsAsync(CancellationToken cancellationToken = default);
}