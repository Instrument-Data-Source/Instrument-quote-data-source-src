using FluentValidation;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Event;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using m = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Validator;

class InstrumentValidator : AbstractValidator<m.Instrument>
{
  public InstrumentValidator()
  {
    RuleFor(e => e.Name).NotEmpty().NotNull().WithEventId(InstrumentValidationEvents.NameIsEmptyEvent);
    RuleFor(e => e.Code).NotEmpty().NotNull().WithEventId(InstrumentValidationEvents.CodeIsEmptyEvent);
    RuleFor(e => e.InstrumentTypeId).Must(e => Enum.IsDefined(typeof(m.InstrumentType.Enum), e)).WithEventId(InstrumentValidationEvents.WrongInstrumentType);
  }
}

