using FluentValidation;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
public class InstrumentValidator : AbstractValidator<ent.Instrument>
{
  public InstrumentValidator()
  {
    RuleFor(e => e).NotNull().WithMessage("Instrument of candle cann't be null")
    .DependentRules(() =>
    {
      RuleFor(e => e.Id).GreaterThan(0).WithMessage("Instrument must have valid Id");
    });
  }
}