namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

public class InstrumentTypeResponseDto : IEquatable<InstrumentTypeResponseDto>
{
  public int Id { get; set; }
  public string Name { get; set; }

  public bool Equals(InstrumentTypeResponseDto? other)
  {
    if (other == null) return false;
    if (this == other) return true;
    return Id == other.Id &&
           Name == other.Name;
  }
}