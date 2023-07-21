using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Shared.Validation;

public static class ValidactionContextExtension
{
  public static bool TryGetProp(this ValidationContext validationContext, string propName, out object? propVal)
  {
    propVal = null;

    var propInfo = validationContext.ObjectType.GetProperty(propName);

    if (propInfo == null)
      return false;

    propVal = propInfo.GetValue(validationContext.ObjectInstance);

    return true;
  }
  public static object? GetProp(this ValidationContext validationContext, string propName)
  {
    if (!TryGetProp(validationContext, propName, out var propVal))
      throw new NullReferenceException($"Property {propName} is not found in {validationContext.ObjectType.FullName}");

    return propVal;
  }
  public static bool TryGetProp<TType>(this ValidationContext validationContext, string propName, out TType propVal)
  {
    propVal = default;
    if (!validationContext.TryGetProp(propName, out object? objPropVal))
    {
      return false;
    }

    if (objPropVal != null)
    {
      propVal = (TType)objPropVal;
      return true;
    }

    Type underlyingType = Nullable.GetUnderlyingType(typeof(TType));

    if (underlyingType == null)
    {
      throw new NullReferenceException($"Property {propName} is Null but type {typeof(TType).Name} doesn't support null value");
    }

    propVal = (TType)objPropVal;
    return true;
  }

  public static TType GetProp<TType>(this ValidationContext validationContext, string propName)
  {
    var propVal = validationContext.GetProp(propName);
    if (propVal != null)
      return (TType)propVal;

    Type underlyingType = Nullable.GetUnderlyingType(typeof(TType));

    if (underlyingType == null)
    {
      throw new NullReferenceException($"Property {propName} is Null but type {typeof(TType).Name} doesn't support null value");
    }

    return (TType)propVal;
  }
}