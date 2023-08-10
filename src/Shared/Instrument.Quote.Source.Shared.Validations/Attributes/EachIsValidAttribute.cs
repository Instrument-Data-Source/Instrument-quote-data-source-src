using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.Shared.Validations.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class EachIsValidAttribute<TValidatedType> : ValidationAttribute
{
  private readonly bool requiresValidationContext;
  private readonly string idName;

  public EachIsValidAttribute(string idName)
  {
    this.requiresValidationContext = this.CheckRequiresValidationContextIn<TValidatedType>();
    this.idName = idName;
  }

  public override bool RequiresValidationContext => requiresValidationContext;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var enumerable = (IEnumerable)value!;
    var allValid = true;
    int idx = -1;
    var Results = new List<ValidationResult>();
    foreach (var item in enumerable)
    {
      idx++;
      if (item == null)
        continue;

      var itemId = item.GetType().GetProperty(idName)!.GetValue(item);

      var itemValid = true;
      var itemResults = new List<ValidationResult>();

      var context = new ValidationContext(item, validationContext, validationContext.Items);
      itemValid &= Validator.TryValidateObject(item, context, itemResults);

      if (!itemValid)
        Results.Add(new ValidationResultExtended("Is invalid", $"{validationContext.MemberName}[{idx}]:{itemId}", itemResults));

      allValid &= itemValid;
    }

    if (allValid)
      return ValidationResult.Success;

    return new ValidationResultExtended("Is invalid", validationContext.MemberName, Results);
  }
}

/*

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class EachAttribute : ValidationAttribute
{
  private readonly bool requiresValidationContext;
  private readonly ValidationAttribute[] attributes;
  private readonly string idName;

  public EachAttribute(string idName, params RequiredAttribute[] usedValidationAttributes)
  {
    //requiresValidationContext = usedValidationAttributes.Any(a => a.RequiresValidationContext);
    this.idName = idName;
  }

  public override bool RequiresValidationContext => requiresValidationContext;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var enumerable = (IEnumerable)value!;
    var allValid = true;
    int idx = 0;
    var Results = new List<ValidationResult>();
    foreach (var item in enumerable)
    {
      var itemId = item.GetType().GetProperty(idName).GetValue(item);

      var itemValid = true;
      var itemResults = new List<ValidationResult>();
      foreach (var validationAtr in attributes)
      {
        var context = new ValidationContext(item, validationContext, validationContext.Items);
        var atrResults = new List<ValidationResult>();
        itemValid &= Validator.TryValidateObject(item, context, atrResults);
        itemResults.AddRange(atrResults);
      }

      if (!itemValid)
        Results.Add(new ValidationResultExtended("Is invalid", $"{validationContext.MemberName}[{idx}]:{itemId}", itemResults));
      allValid &= itemValid;
      idx++;
    }

    if (allValid)
      return ValidationResult.Success;

    return new ValidationResultExtended("Is invalid", validationContext.MemberName, Results);
  }
}
*/