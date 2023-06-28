using FluentValidation;
using Instrument.Quote.Source.App.Core.Event;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using m = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Validator.Instrument;

class TypeValidator : AbstractValidator<int>
{
  public TypeValidator()
  {
    RuleFor(e => e)
      .Must(e => Enum.IsDefined(typeof(m.InstrumentType.Enum), e)).WithEventId(ValidationEvents.WrongInstrumentType);
  }
}