using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class GreaterThenFromDateAttribute : ValidationAttribute
{

  public override bool RequiresValidationContext => true;

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var period = (LoadedPeriod)validationContext.ObjectInstance;
    var dt = (DateTime)value;
    
    if (dt > period.FromDate)
      return ValidationResult.Success;

    return new ValidationResult($"Value must be greater than {nameof(period.FromDate)}", new string[] { validationContext.MemberName! });
  }
}