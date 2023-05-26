
using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;

public class NewInstrumentRequestDto // : IValidatableObject
{
  public string Name { get; set; }
  public string Code { get; set; }
  public int TypeId { get; set; }
  public byte PriceDecimalLen { get; set; }
  public byte VolumeDecimalLen { get; set; }

}