namespace Instrument.Quote.Source.App.Core.ChartAggregate.Dto;

public class ChartDto : IEquatable<ChartDto>
{
  public int InstrumentId { get; set; }
  public int TimeFrameId { get; set; }
  public DateTime FromDate { get; set; }
  public DateTime UntillDate { get; set; }

  public bool Equals(ChartDto? other)
  {
    if (other == null) return false;
    if (this == other) return true;
    return
      FromDate.Equals(other.FromDate) &&
      UntillDate.Equals(other.UntillDate) &&
      InstrumentId.Equals(other.InstrumentId) &&
      TimeFrameId.Equals(other.TimeFrameId);
  }
}