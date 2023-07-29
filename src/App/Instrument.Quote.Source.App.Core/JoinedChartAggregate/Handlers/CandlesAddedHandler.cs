using Instrument.Quote.Source.App.Core.ChartAggregate.Events;
using MediatR;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Handlers;

public class CandlesAddedHandler : INotificationHandler<CandlesAddedNotification>
{
  public async Task Handle(CandlesAddedNotification notification, CancellationToken cancellationToken)
  {
    await Task.CompletedTask;
  }
}