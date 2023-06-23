using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
public class CandleDto : IEquatable<CandleDto>
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


  public class Validator : AbstractValidator<CandleDto>
  {
    public Validator(TimeFrame.Enum timeFrame)
    {
      var dtValidator = new CandleDateTimeValidator(timeFrame);
      RuleFor(e => e.DateTime)
        .SetValidator(c => dtValidator)
          .WithMessage("Candle in candle is invalid");
      RuleFor(e => e.High)
        .GreaterThanOrEqualTo(e => e.Open)
        .WithMessage("High must be GE Open.");
      RuleFor(e => e.High)
        .GreaterThanOrEqualTo(e => e.Close)
          .WithMessage("High must be GE Close.");
      RuleFor(e => e.Low)
        .LessThanOrEqualTo(e => e.Open)
          .WithMessage("Low must be LE Open.");
      RuleFor(e => e.Low)
        .LessThanOrEqualTo(e => e.Close)
          .WithMessage("Low must be LE Close.");
      RuleFor(e => e.Volume)
        .GreaterThanOrEqualTo(0)
          .WithMessage("Volume must be GE 0.");
    }
  }
}

