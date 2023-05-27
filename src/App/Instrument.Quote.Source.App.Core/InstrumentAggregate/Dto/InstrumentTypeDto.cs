namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

public class InstrumentTypeDto : IEquatable<InstrumentTypeDto>
{
  public int Id { get; set; }
  public string Name { get; set; }

  public bool Equals(InstrumentTypeDto? other)
  {
    if (other == null) return false;
    if (this == other) return true;
    return Id == other.Id &&
           Name == other.Name;
  }
}