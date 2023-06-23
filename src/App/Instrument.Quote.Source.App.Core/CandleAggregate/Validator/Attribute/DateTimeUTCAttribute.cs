using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class UTCKindAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => false;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var dt = (DateTime)value;
    if (dt.Kind == DateTimeKind.Utc)
      return ValidationResult.Success;

    return new ValidationResult($"DateTime must be in UTC kind", new string[] { validationContext.MemberName! });
  }
}