using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Wrapper;
public abstract class AbsWrapperAttribute<TWrappedValidationAttribute> : ValidationAttribute where TWrappedValidationAttribute : ValidationAttribute
{
  protected readonly ValidationAttribute wrappedAttribute;
  public AbsWrapperAttribute(ValidationAttribute wrappedAttribute)
  {
    this.wrappedAttribute = wrappedAttribute;
  }
  public AbsWrapperAttribute(params object[] wrappedValidationAtrCostrPar) : this(CreateInstance(wrappedValidationAtrCostrPar))
  {
  }
  public override bool RequiresValidationContext => wrappedAttribute.RequiresValidationContext;

  protected static TWrappedValidationAttribute CreateInstance(object[] constructorParameters)
  {
    var wrappedType = typeof(TWrappedValidationAttribute);
    var constructor = typeof(TWrappedValidationAttribute).GetConstructor(GetParameterTypes(constructorParameters));
    if (constructor == null)
      constructor = typeof(TWrappedValidationAttribute).GetConstructor(new Type[] { constructorParameters.GetType() });


    if (constructor != null)
      return (TWrappedValidationAttribute)constructor.Invoke(constructorParameters);
    else
      throw new InvalidOperationException("Constructor not found for the specified type.");
  }

  protected static Type[] GetParameterTypes(object[] parameters)
  {
    return parameters.Select(p => p.GetType()).ToArray();
  }
}