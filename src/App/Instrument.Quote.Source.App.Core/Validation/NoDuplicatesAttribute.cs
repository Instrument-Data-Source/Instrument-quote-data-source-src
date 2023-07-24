
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class NoDuplicatesAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => false;

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value == null)
      return ValidationResult.Success;

    var enumerable = (IEnumerable)value;
    if (enumerable == null)
      return ValidationResult.Success;

    if (!enumerable.Cast<object>().Any())
      return ValidationResult.Success;

    var compValues = enumerable.Cast<IComparable>();

    if (compValues.Distinct().Count() != compValues.Count())
      return new ValidationResult($"Must contaion only unique values", new string[] { validationContext.MemberName! });

    return ValidationResult.Success;
  }
}