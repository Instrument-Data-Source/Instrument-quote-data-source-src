using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;

public class TimeFrameResponseDto : IEquatable<TimeFrameResponseDto>
{
  public TimeFrameResponseDto() { }
  public TimeFrameResponseDto(TimeFrame entity)
  {
    Id = entity.Id;
    Code = entity.Name;
    Seconds = entity.Seconds;
  }

  public int Id { get; set; }
  public string Code { get; set; }
  public int Seconds { get; set; }

  public bool Equals(TimeFrameResponseDto? other)
  {
    return other != null &&
      other.Id == this.Id &&
      other.Code == this.Code &&
      other.Seconds == this.Seconds;
  }
}