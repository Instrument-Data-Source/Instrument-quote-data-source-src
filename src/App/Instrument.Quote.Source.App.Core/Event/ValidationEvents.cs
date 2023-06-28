using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.App.Core.Event;
/// <summary>
/// 10000-11999
/// </summary>
public static class ValidationEvents
{

  public static EventId IsEmptyEvent => new EventId(10000, "Value is empty");
  public static EventId WrongInstrumentType => new EventId(10001, "Invalid instrument type");
  public static EventId IsTooLongEvent => new EventId(10002, "Value is too long");
  public static EventId IdNotFoundEvent => new EventId(10003, "Id not found");
  public static EventId FromGeUntillEvent => new EventId(10004, "From date greater or equal than less date");
  public static EventId CandleDateIsOutOfFromAndUntillEvent => new EventId(10005, "Date is out of range From date and Untill date");
}