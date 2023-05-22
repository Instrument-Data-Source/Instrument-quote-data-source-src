using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;

public static class EntityToDtoMapper
{
  public static TimeFrameResponseDto ToDto(this TimeFrame timeFrame)
  {
    return new TimeFrameResponseDto(timeFrame);
  }
}