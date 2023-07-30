using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.Event;
/// <summary>
/// 20000-21999
/// </summary>
public static class BusinessEvents
{
  public static EventId AddNewCandles => new EventId(20000, "Value is empty");
}