using MediatR;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Events;

public class JoinedChartUpdateRequested : INotification
{
  public readonly int JoinedChartId;
  public bool background = true;

  public JoinedChartUpdateRequested(int joinedChartId, bool background = true)
  {
    JoinedChartId = joinedChartId;
    this.background = background;
  }
}