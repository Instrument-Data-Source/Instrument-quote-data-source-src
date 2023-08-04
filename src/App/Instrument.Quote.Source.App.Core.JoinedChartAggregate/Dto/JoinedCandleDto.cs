using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.Validation;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;

public class JoinedCandleDto : CandleDto, IEquatable<JoinedCandleDto>
{
  [Required]
  [UTCKind]
  public DateTime TargetDateTime { get; set; }
  public bool IsLast { get; set; }
  public bool IsFullCalced { get; set; }

  public bool Equals(JoinedCandleDto? other)
  {
    return base.Equals(other) &&
           this.TargetDateTime == other.TargetDateTime &&
           this.IsLast == other.IsLast;
  }
}