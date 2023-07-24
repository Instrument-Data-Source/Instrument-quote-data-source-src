using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Shared.Validation;

namespace Instrument.Quote.Source.App.Core.Validation;

public abstract class absDependableOfPropAttribute : ValidationAttribute
{
  private readonly string usedIdPropName;
  private readonly bool? useContextItems;
  /// <summary>
  /// 
  /// </summary>
  /// <param name="usedIdPropName">name of property which contain Id of entity</param>
  /// <param name="useContextItems">does property contain in ValidationContext.Items?</param>
  public absDependableOfPropAttribute(string usedIdPropName, bool? useContextItems = null)
  {
    this.usedIdPropName = usedIdPropName;
    this.useContextItems = useContextItems;
  }
  protected int getId(ValidationContext validationContext)
  {
    if (useContextItems == null || useContextItems == true)
    {
      if (validationContext.Items.TryGetValue(usedIdPropName, out var retObj))
        return (int)retObj!;
      else if (useContextItems == true)
        throw new KeyNotFoundException($"Cannot find {usedIdPropName} in ValidationContext Items");
    }

    if (useContextItems == null || useContextItems == false)
    {
      if (validationContext.TryGetProp<int>(usedIdPropName, out int propVal))
        return propVal;
      else if (useContextItems == false)
        throw new ArgumentException($"Cann't find property {usedIdPropName} in object {validationContext.ObjectType.Name}");
    }

    throw new ArgumentException($"Cann't find property {usedIdPropName} for {this.GetType().Name}");
  }
}
