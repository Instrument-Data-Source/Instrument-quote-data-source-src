using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Event;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using m = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Validator.Instrument;

class CodeValidator : AbstractValidator<string>
{
  public CodeValidator()
  {
    RuleFor(e => e)
      .Must(e =>
      {
        return !string.IsNullOrEmpty(e);
      }).WithEventId(InstrumentValidationEvents.IsEmptyEvent)
      .Length(1, 10).WithEventId(InstrumentValidationEvents.IsTooLongEvent);
  }
}