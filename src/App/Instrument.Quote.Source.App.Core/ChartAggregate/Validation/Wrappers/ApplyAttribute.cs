using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Wrapper;

/// <summary>
/// Apply validation attribute to property inside object
/// </summary>
/// <example>
/// Apply Range(3,100) to User.Level if user setted to Machine
/// 
/// public class User
/// {
///   public int Id {get;set;}  
///   public int Level {get;set;}
/// }
/// 
/// public Machine
/// {  
///   [Apply<Range>("Level", new object []{3,100})]
///   public User user {get;set;}
/// }
/// </example>
/// <typeparam name="TApplyValidationAttribute">Applied validation attribute</typeparam>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = true)]
public class Apply<TApplyValidationAttribute> : AbsWrapperAttribute<TApplyValidationAttribute> where TApplyValidationAttribute : ValidationAttribute
{
  private readonly string propName;
  public Apply(string propName, params object[] applyValidationAtrCostrPar) : base(applyValidationAtrCostrPar)
  {
    this.propName = propName;
  }
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    if (value == null)
      return ValidationResult.Success;
    object? validatedProp = GetObjProp(value, propName);

    var itemResults = new List<ValidationResult>();
    var context = new ValidationContext(validationContext.ObjectInstance, validationContext, validationContext.Items);
    context.MemberName = propName;

    var result = wrappedAttribute.GetValidationResult(validatedProp, context);

    return result;
  }


}