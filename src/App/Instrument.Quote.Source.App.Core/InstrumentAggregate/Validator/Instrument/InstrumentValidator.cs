using FluentValidation;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Event;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using m = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Validator.Instrument;

class InstrumentValidator : AbstractValidator<m.Instrument>
{
  public InstrumentValidator()
  {
    RuleFor(e => e.Name).NotNull().SetValidator(e => new NameValidator());
    RuleFor(e => e.Code).NotNull().SetValidator(e => new CodeValidator());
    RuleFor(e => e.InstrumentTypeId).SetValidator(e => new TypeValidator());
  }
}


