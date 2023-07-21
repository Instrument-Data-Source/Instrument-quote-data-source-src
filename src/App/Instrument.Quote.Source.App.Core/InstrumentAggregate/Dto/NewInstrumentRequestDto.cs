
using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

public class NewInstrumentRequestDto: IEquatable<NewInstrumentRequestDto>
{
  [MaxLength(50)]
  [MinLength(1)]
  public string Name { get; set; }
  [MaxLength(10)]
  [MinLength(1)]
  public string Code { get; set; }

  public int TypeId { get; set; }
  public byte PriceDecimalLen { get; set; }
  public byte VolumeDecimalLen { get; set; }

  public bool Equals(NewInstrumentRequestDto? other)
  {
    if (other == null) return false;
    if (this == other) return true;
    return Name == other.Name &&
           Code == other.Code &&
           TypeId == other.TypeId &&
           PriceDecimalLen == other.PriceDecimalLen &&
           VolumeDecimalLen == other.VolumeDecimalLen;
  }
}