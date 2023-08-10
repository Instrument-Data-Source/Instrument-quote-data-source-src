using MediatR;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Events;

public class EventRequestJoinedChartUpdate : INotification
{
  public readonly int JoinedChartId;
  public bool background = true;

  public EventRequestJoinedChartUpdate(int joinedChartId, bool background = true)
  {
    JoinedChartId = joinedChartId;
    this.background = background;
  }
}