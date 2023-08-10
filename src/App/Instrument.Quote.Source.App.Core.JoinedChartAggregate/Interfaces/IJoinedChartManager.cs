using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
public interface IJoinedChartManager
{
  Task UpdateAsync(int joinedChartId, CancellationToken cancellationToken = default);
}