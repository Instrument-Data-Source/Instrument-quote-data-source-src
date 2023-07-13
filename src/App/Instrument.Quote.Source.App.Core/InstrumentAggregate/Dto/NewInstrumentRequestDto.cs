
using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

public class NewInstrumentRequestDto // : IValidatableObject
{
  [MaxLength(5)]
  public string Name { get; set; }
  [MaxLength(5)]
  public string Code { get; set; }

  public int TypeId { get; set; }
  public byte PriceDecimalLen { get; set; }
  public byte VolumeDecimalLen { get; set; }

}