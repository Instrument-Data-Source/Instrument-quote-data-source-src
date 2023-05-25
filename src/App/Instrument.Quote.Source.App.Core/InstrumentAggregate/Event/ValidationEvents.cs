using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Event;
/// <summary>
/// 11000-11099
/// </summary>
public static class InstrumentValidationEvents
{

  public static EventId IsEmptyEvent => new EventId(11000, "Value is empty");
  public static EventId WrongInstrumentType => new EventId(11001, "Invalid instrument type");
  public static EventId IsTooLongEvent => new EventId(11002, "Value is too long");
}