using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
public class CandleDto : IEquatable<CandleDto>, Candle.IPayload
{
  public DateTime DateTime { get; set; }
  public decimal Open { get; set; }
  public decimal High { get; set; }
  public decimal Low { get; set; }
  public decimal Close { get; set; }
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

}

