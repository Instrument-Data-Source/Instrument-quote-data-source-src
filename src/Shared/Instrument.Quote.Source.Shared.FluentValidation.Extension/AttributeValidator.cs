using FluentValidation;

namespace Instrument.Quote.Source.Shared.FluentValidation.Extension;

public class PropertyValidator<T> : AbstractValidator<T>
{
  public PropertyValidator()
  {
    //RuleFor(e => e).Must(e => e.GetType().Attributes)
  }
}