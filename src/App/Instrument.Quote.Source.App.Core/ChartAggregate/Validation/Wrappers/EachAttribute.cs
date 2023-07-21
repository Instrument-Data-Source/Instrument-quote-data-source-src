using System.Collections;
using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Wrapper;
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
  // private readonly string idName;
  public Each(TEachValidationAttribute eachValidationAttribute) : base(eachValidationAttribute)
  {
    //this.idName = idName;
  }
  public Each(params object[] eachValidationAtrCostrPar) : base(eachValidationAtrCostrPar)
  {
    //this.idName = idName;
  }

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

      //var itemId = item.GetType().GetProperty(idName)!.GetValue(item);

      var itemResults = new List<ValidationResult>();
      var result = wrappedAttribute.GetValidationResult(item, validationContext);

      if (result != ValidationResult.Success)
      {
        Results.Add(result!);
        allValid = false;
      }
    }

    if (allValid)
      return ValidationResult.Success;

    return new ValidationResultExtended("Is invalid", validationContext.MemberName!, Results);
  }
}
