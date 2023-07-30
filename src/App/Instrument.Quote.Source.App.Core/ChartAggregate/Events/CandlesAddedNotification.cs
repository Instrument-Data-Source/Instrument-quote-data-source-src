using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using MediatR;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Events;

public class CandlesAddedNotification : INotification
{

  public readonly Chart Chart;
  public readonly DateTime FromDateTime;
  public readonly DateTime UntillDateTime;

  public CandlesAddedNotification(Chart chart, DateTime fromDateTime, DateTime untillDateTime)
  {
    Chart = chart;
    FromDateTime = fromDateTime;
    UntillDateTime = untillDateTime;
  }
}