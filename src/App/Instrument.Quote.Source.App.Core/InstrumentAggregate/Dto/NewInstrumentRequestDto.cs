
using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

public class NewInstrumentRequestDto // : IValidatableObject
{
  [Required]
  public string Name { get; set; }
  [Required]
  public string Code { get; set; }
  public string? Type { get; set; }
  public int TypeId { get; set; }
  public byte PriceDecimalLen { get; set; }
  public byte VolumeDecimalLen { get; set; }

}