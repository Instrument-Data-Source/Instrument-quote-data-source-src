using FluentValidation;
namespace Instrument.Quote.Source.Shared.FluentValidation.Extension.Test.TestData;

public class CustomValidator : AbstractValidator<Entity>
{
  public CustomValidator()
  {
    RuleFor(e => e.Value1).GreaterThan(5);
    RuleFor(e => e.SubEntity1).SetValidator(e => new CustomValidator2());
  }
}


public class CustomValidator2 : AbstractValidator<SubEntity>
{
  public CustomValidator2()
  {
    RuleFor(e => e.Value2).Must(e => e.Contains("e"));
    RuleFor(e => e.Value3).Must(e => e).WithMessage("Must bu True");
  }
}