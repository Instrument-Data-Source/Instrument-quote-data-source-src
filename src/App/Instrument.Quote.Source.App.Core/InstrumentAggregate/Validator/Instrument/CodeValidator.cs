using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Instrument.Quote.Source.App.Core.Event;
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
      }).WithEventId(ValidationEvents.IsEmptyEvent)
      .Length(1, 10).WithEventId(ValidationEvents.IsTooLongEvent);
  }
}