using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LessThenUntillDateAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => true;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var period = (LoadedPeriod)validationContext.ObjectInstance;
    var dt = (DateTime)value;

    if (dt < period.UntillDate)
      return ValidationResult.Success;

    return new ValidationResult($"Value must be less than {nameof(period.UntillDate)}", new string[] { validationContext.MemberName! });
  }
}