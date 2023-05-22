namespace Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
public class PeriodResponseDto : IEquatable<PeriodResponseDto>
{
  public DateTime? FromDate { get; set; }
  public DateTime? UntillDate { get; set; }

  public bool Equals(PeriodResponseDto? other)
  {
    if (other == null) return false;
    if (this == other) return true;
    return FromDate == other.FromDate &&
           UntillDate == other.UntillDate;
  }
}