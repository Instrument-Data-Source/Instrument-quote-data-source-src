using MediatR;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Events;

public class JoinedChartUpdateRequested : INotification
{
  public readonly int JoinedChartId;

  public JoinedChartUpdateRequested(int joinedChartId)
  {
    JoinedChartId = joinedChartId;
  }
}