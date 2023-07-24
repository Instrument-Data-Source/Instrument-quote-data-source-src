using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Instrument.Quote.Source.App.Core.Validation;
public static class ValidationAttributeExtension
{
  public static bool CheckRequiresValidationContextIn<TValidatedType>()
  {
    var checkedType = typeof(TValidatedType);
    return checkedType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .SelectMany(p => p.GetCustomAttributes<ValidationAttribute>(inherit: true))
            .Any(a => a.RequiresValidationContext) &&
           checkedType.GetFields(BindingFlags.Public | BindingFlags.Instance)
            .SelectMany(p => p.GetCustomAttributes<ValidationAttribute>(inherit: true))
            .Any(a => a.RequiresValidationContext);
  }
  public static bool CheckRequiresValidationContextIn<TValidatedType>(this ValidationAttribute validationAttribute)
  {
    return CheckRequiresValidationContextIn<TValidatedType>();
  }
}