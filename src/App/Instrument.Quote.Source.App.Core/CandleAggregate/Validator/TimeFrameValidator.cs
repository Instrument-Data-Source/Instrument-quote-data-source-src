using FluentValidation;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
public class TimeFrameValidator : AbstractValidator<TimeFrame>
{
  public TimeFrameValidator()
  {
    RuleFor(e => e).NotNull().WithMessage("TimeFrame of candle cann't be null")
    .DependentRules(() =>
    {
      RuleFor(e => e.Id).GreaterThan(0).WithMessage("TimeFrame must have valid Id");
    });
  }
}