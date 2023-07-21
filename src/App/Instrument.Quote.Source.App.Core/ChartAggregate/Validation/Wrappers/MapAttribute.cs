using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Wrapper;

/// <summary>
/// Map property from IEnumerable<object> to pass it into validation attribute
/// </summary>
/// <example>
/// Map IEnumerable<User> into IEnumerable<int> (IEnumerable of Levels) 
/// and apply SumInListMore(3) attribute to new list
/// 
/// public class User
/// {
///   public int Id {get;set;}  
///   public int Level {get;set;}
/// }
/// 
/// public Machine
/// {  
///   [Map<SumInListMore>("Level", new object []{3})]
///   public User user {get;set;}
/// }
/// </example>
/// <typeparam name="TMapValidationAttribute">Applied validation attribute</typeparam>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
public class Map<TMapValidationAttribute> : AbsWrapperAttribute<TMapValidationAttribute> where TMapValidationAttribute : ValidationAttribute
{
  private readonly string propName;
  public Map(string propName) : this(propName, new object[] { })
  {
  }
  public Map(string propName, params object[] mapValidationAtrCostrPar) : base(mapValidationAtrCostrPar)
  {
    this.propName = propName;
  }
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value == null)
      return ValidationResult.Success;
    var enumerable = (IEnumerable)value!;

    var mappedEnumerable = mapValue(enumerable, propName);

    var itemResults = new List<ValidationResult>();
    var result = wrappedAttribute.GetValidationResult(mappedEnumerable, validationContext);
    return result;
  }

  public IEnumerable<object> mapValue(IEnumerable objects, string propertyName)
  {
    foreach (var obj in objects)
    {
      Type objectType = obj.GetType();
      PropertyInfo propertyInfo = objectType.GetProperty(propertyName);

      if (propertyInfo != null)
      {
        yield return propertyInfo.GetValue(obj)!;
      }
      else
      {
        throw new ArgumentException($"Property '{propertyName}' does not exist in type '{objectType.Name}'");
      }
    }
  }

}