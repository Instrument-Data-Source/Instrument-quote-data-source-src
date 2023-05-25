using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Event;
/// <summary>
/// 11000-11099
/// </summary>
public static class InstrumentValidationEvents
{

  public static EventId NameIsEmptyEvent => new EventId(11000, "Name is empty");
  public static EventId CodeIsEmptyEvent => new EventId(11001, "Code is empty");
  public static EventId WrongInstrumentType => new EventId(11002, "Wrong event type");
}