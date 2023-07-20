using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Dto;

public class CandleDto : IEquatable<CandleDto>
{
  [Required]
  [UTCKind]
  public DateTime DateTime { get; set; }
  [Required]
  [CompareTo(CompType.GE, nameof(Low))]
  [CompareTo(CompType.LE, nameof(High))]
  public decimal Open { get; set; }
  [Required]
  [CompareTo(CompType.GE, nameof(Open), nameof(Low), nameof(Close))]
  public decimal High { get; set; }
  [Required]
  [CompareTo(CompType.LE, nameof(Open), nameof(High), nameof(Close))]
  public decimal Low { get; set; }
  [Required]
  [CompareTo(CompType.GE, nameof(Low))]
  [CompareTo(CompType.LE, nameof(High))]
  public decimal Close { get; set; }
  [Required]
  [Range(0, int.MaxValue)]
  public decimal Volume { get; set; }

  public bool Equals(CandleDto? other)
  {
    if (other == null) return false;
    if (this == other) return true;
    return
      DateTime.Equals(other.DateTime) &&
      Open.Equals(other.Open) &&
      High.Equals(other.High) &&
      Low.Equals(other.Low) &&
      Close.Equals(other.Close) &&
      Volume.Equals(other.Volume);
  }

  public bool IsDecimalPartFit(IDecimalPartLongChecker checker)
  {
    return checker.IsPriceDecPartFit(Open) &&
           checker.IsPriceDecPartFit(High) &&
           checker.IsPriceDecPartFit(Low) &&
           checker.IsPriceDecPartFit(Close) &&
           checker.IsVolumeDecPartFit(Volume);
  }
}