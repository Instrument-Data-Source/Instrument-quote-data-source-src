using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.Validation;
public enum CompType
{
  LT,
  LE,
  EQ,
  GE,
  GT
}
[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class CompareToAttribute : ValidationAttribute
{
  private readonly CompType compareType;
  private readonly string[] propNames;
  private readonly Func<int, bool> compareFunc;
  public CompareToAttribute(CompType compareType, params string[] propName)
  {
    this.compareType = compareType;
    this.propNames = propName;
    switch (compareType)
    {
      case CompType.LT:
        compareFunc = (res) => res < 0;
        break;
      case CompType.LE:
        compareFunc = (res) => res <= 0;
        break;
      case CompType.EQ:
        compareFunc = (res) => res == 0;
        break;
      case CompType.GE:
        compareFunc = (res) => res >= 0;
        break;
      case CompType.GT:
        compareFunc = (res) => res > 0;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(compareType), Enum.GetName<CompType>(compareType), "Unexpected value");
    }
  }
  public override bool RequiresValidationContext => true;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var obj = validationContext.ObjectInstance;
    var valueComp = value as IComparable;
    foreach (var propName in propNames)
    {
      var compValue = validationContext.ObjectType.GetProperty(propName)!.GetValue(obj) as IComparable;
      if (!compareFunc(valueComp!.CompareTo(compValue)))
        return new ValidationResult($"Value must be {Enum.GetName<CompType>(compareType)} than {propName}", new string[] { validationContext.MemberName! });
    }

    return ValidationResult.Success;
  }
}