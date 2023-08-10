using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
public interface IJoinedChartFactory
{
  Task<JoinedChart> CreateNewFor(int chartId, int targetTimeFrameId, CancellationToken cancellationToken);
}