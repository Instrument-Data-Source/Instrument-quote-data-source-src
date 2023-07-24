using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.Validation.Wrapper;
/// <summary>
/// Apply validation attribute to each element inside IEnumerable property
/// </summary>
/// <example>
/// Apply LewelBetween(3,100) to each User in users of Machine
/// 
/// public class User
/// {
///   public int Id {get;set;}  
///   public int Level {get;set;}
/// }
/// 
/// public Machine
/// {  
///   [Each<LevelBetween>("Id", new object []{3,100})]
///   public List<User> users {get;set;}
/// }
/// </example>
/// <typeparam name="TEachValidationAttribute">Applied validation attribute</typeparam>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
public class Each<TEachValidationAttribute> : AbsWrapperAttribute<TEachValidationAttribute> where TEachValidationAttribute : ValidationAttribute
{
  private readonly bool eachMust = false;

  public Each(TEachValidationAttribute eachValidationAttribute) : base(eachValidationAttribute)
  { }
  public Each(params object[] eachValidationAtrCostrPar) : base(eachValidationAtrCostrPar)
  { }

  protected Each(bool eachMust, TEachValidationAttribute eachValidationAttribute) : base(eachValidationAttribute)
  {
    this.eachMust = eachMust;
  }
  protected Each(bool eachMust, params object[] eachValidationAtrCostrPar) : base(eachValidationAtrCostrPar)
  {
    this.eachMust = eachMust;
  }

  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value == null)
      return ValidationResult.Success;

    var enumerable = (IEnumerable)value;
    if (enumerable == null)
      throw new ArgumentException($"Validation object '{validationContext.MemberName}' is not IEnumerable");

    var allValid = true;
    int idx = -1;
    var Results = new Dictionary<int, ValidationResult>();
    foreach (var item in enumerable)
    {
      idx++;
      if (!eachMust && item == null)
        continue;

      //var itemId = item.GetType().GetProperty(idName)!.GetValue(item);

      var itemResults = new List<ValidationResult>();
      var result = wrappedAttribute.GetValidationResult(item, validationContext);

      if (result != ValidationResult.Success)
      {
        Results[idx] = result!;
        allValid = false;
      }
    }

    if (allValid)
      return ValidationResult.Success;

    var detailedText = string.Join(", ", Results.Select(kvp => $"[{kvp.Key}].{kvp.Value.MemberNames}: {kvp.Value.ErrorMessage}"));
    return new ValidationResult($"Elements is invalid: {detailedText}", new[] { validationContext.MemberName! });
  }
}
