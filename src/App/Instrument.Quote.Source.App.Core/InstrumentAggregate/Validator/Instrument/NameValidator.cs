using FluentValidation;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Event;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using m = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Validator.Instrument;

class NameValidator : AbstractValidator<string>
{
  public NameValidator()
  {
    RuleFor(e => e)
      .Must(e => string.IsNullOrEmpty(e)).WithEventId(InstrumentValidationEvents.IsEmptyEvent)
      .Length(1, 50).WithEventId(InstrumentValidationEvents.IsTooLongEvent);
  }
}