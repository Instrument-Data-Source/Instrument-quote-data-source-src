using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
public class BetweenAttribute : ValidationAttribute
{
  private readonly Func<int, bool> compareLeftFunc;
  private readonly Func<int, bool> compareRightFunc;
  private readonly string leftSide;
  private readonly string rightSide;
  private readonly CompType compareLeft;
  private readonly CompType compareRight;

  public BetweenAttribute(CompType compareLeft, string leftSide, CompType compareRight, string rightSide)
  {
    switch (compareLeft)
    {
      case CompType.EQ:
        compareLeftFunc = (res) => res == 0;
        break;
      case CompType.GE:
        compareLeftFunc = (res) => res >= 0;
        break;
      case CompType.GT:
        compareLeftFunc = (res) => res > 0;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(compareLeft), Enum.GetName<CompType>(compareLeft), "Side doesn't suppert this value");
    }

    switch (compareRight)
    {
      case CompType.LT:
        compareRightFunc = (res) => res < 0;
        break;
      case CompType.LE:
        compareRightFunc = (res) => res <= 0;
        break;
      case CompType.EQ:
        compareRightFunc = (res) => res == 0;
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(compareRight), Enum.GetName<CompType>(compareRight), "Side doesn't suppert this value");
    }

    this.leftSide = leftSide;
    this.rightSide = rightSide;
    this.compareLeft = compareLeft;
    this.compareRight = compareRight;
  }
  public override bool RequiresValidationContext => true;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var obj = validationContext.ObjectInstance;
    var valueComp = value as IComparable;

    var leftValue = validationContext.ObjectType.GetProperty(leftSide)!.GetValue(obj) as IComparable;
    var rightValue = validationContext.ObjectType.GetProperty(rightSide)!.GetValue(obj) as IComparable;
    if (!compareLeftFunc(valueComp!.CompareTo(leftValue)) || !compareRightFunc(valueComp!.CompareTo(rightValue)))
      return new ValidationResult($"Value must be {Enum.GetName<CompType>(compareLeft)} than {leftSide} and {Enum.GetName<CompType>(compareRight)} than {rightSide}", new string[] { validationContext.MemberName! });

    return ValidationResult.Success;
  }
}
