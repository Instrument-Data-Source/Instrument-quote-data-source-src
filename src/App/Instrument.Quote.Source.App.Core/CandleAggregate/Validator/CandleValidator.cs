using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
public class CandleValidator : AbstractValidator<Candle>
{
  public CandleValidator(TimeFrame.Enum tfEnum)
  {
    RuleLevelCascadeMode = CascadeMode.Continue;
    var dt_validator = new CandleDateTimeValidator(tfEnum);
    RuleFor(e => e.DateTime)
      .SetValidator(c => new CandleDateTimeValidator((TimeFrame.Enum)c.TimeFrameId))
        .WithMessage("Candle in candle is invalid");
    RuleFor(e => e.HighStore)
      .GreaterThanOrEqualTo(e => e.OpenStore)
      .WithMessage("High must be GE Open.");
    RuleFor(e => e.HighStore)
      .GreaterThanOrEqualTo(e => e.CloseStore)
        .WithMessage("High must be GE Close.");
    RuleFor(e => e.LowStore)
      .LessThanOrEqualTo(e => e.OpenStore)
        .WithMessage("Low must be LE Open.");
    RuleFor(e => e.LowStore)
      .LessThanOrEqualTo(e => e.CloseStore)
        .WithMessage("Low must be LE Close.");
    RuleFor(e => e.VolumeStore)
      .GreaterThanOrEqualTo(0)
        .WithMessage("Volume must be GE 0.");
  }
}