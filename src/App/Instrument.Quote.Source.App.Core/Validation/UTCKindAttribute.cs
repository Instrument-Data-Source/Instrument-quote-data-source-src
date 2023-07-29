using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class UTCKindAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => false;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var dt = (DateTime)value;
    if (IsUTC(dt))
      return ValidationResult.Success;

    return new ValidationResult($"DateTime must be in UTC kind", new string[] { validationContext.MemberName! });
  }

  public static bool IsUTC(DateTime dt)
  {
    return dt.Kind == DateTimeKind.Utc;
  }

  public static bool IsNotUTC(DateTime dt)
  {
    return !IsUTC(dt);
  }
}
