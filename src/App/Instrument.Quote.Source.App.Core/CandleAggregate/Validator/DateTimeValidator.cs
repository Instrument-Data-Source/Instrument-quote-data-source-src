using FluentValidation;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;

public class DateTimeValidator : AbstractValidator<DateTime>
{
  public DateTimeValidator()
  {
    RuleFor(e => e).NotEmpty().WithMessage("Must be filled");
    RuleFor(e => e).Must(e => e.Kind == DateTimeKind.Utc).WithMessage("Must be in UTC type");
  }
}
