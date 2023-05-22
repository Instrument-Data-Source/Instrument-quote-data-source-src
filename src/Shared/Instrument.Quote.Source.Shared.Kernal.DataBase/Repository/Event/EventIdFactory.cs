using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Event;

public static class EventIdFactory
{
  public static EventId GetEventId(this EventEnum event_enum)
  {
    return new EventId((int)event_enum, Enum.GetName<EventEnum>(event_enum));
  }
}