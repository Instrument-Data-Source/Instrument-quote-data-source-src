using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Instrument.Quote.Source.Shared.Validations.Attributes;


[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class IsValidAttribute<TValidatedType> : ValidationAttribute
{
  private readonly bool requiresValidationContext;

  public IsValidAttribute(Type validatedType)
  {
    this.requiresValidationContext = this.CheckRequiresValidationContextIn<TValidatedType>();
  }

  public override bool RequiresValidationContext => requiresValidationContext;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var context = new ValidationContext(value, validationContext, validationContext.Items);
    var result = new List<ValidationResult>();
    var IsValid = Validator.TryValidateObject(value, context, result);
    if (IsValid)
      return ValidationResult.Success;

    return new ValidationResultExtended("Is invalid", validationContext.MemberName, result);
  }
  /*
    public static bool CheckRequiresValidationContextIn()
    {
      var checkedType = typeof(TValidatedType);
      return checkedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
              .SelectMany(p => p.GetCustomAttributes<ValidationAttribute>(inherit: true))
              .Any(a => a.RequiresValidationContext) &&
             checkedType.GetFields(BindingFlags.Public | BindingFlags.Instance)
              .SelectMany(p => p.GetCustomAttributes<ValidationAttribute>(inherit: true))
              .Any(a => a.RequiresValidationContext);
    }
    */
}