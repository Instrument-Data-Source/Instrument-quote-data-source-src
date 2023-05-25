using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Event;
/// <summary>
/// 10000-10099
/// </summary>
public static class CommonEvent
{

  public static EventId ApplicationErrorEvent => new EventId(10000, "Application exception");
}