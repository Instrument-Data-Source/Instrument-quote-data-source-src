using FluentValidation;
using FluentValidation.Internal;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;
public partial class Candle
{
  protected override FluentValidation.Results.ValidationResult ValidateSelf(Action<ValidationStrategy<Candle>> options) =>
     new CandleValidator().Validate(this, options);

  public class CandleValidator : AbstractValidator<Candle>
  {
    public CandleValidator()
    {
      RuleFor(e => e).SetValidator(new CandlePayloadValidator());
    }
  }
  public class CandlePayloadValidator : AbstractValidator<IPayload>
  {
    public CandlePayloadValidator()
    {
      RuleFor(e => e.DateTime).SetValidator(new DateTimeValidator());
      RuleFor(e => e.Open).SetValidator((c, p) => new OpenPriceValidator(c));
      RuleFor(e => e.High).SetValidator((c, p) => new HighPriceValidator(c));
      RuleFor(e => e.Low).SetValidator((c, p) => new LowPriceValidator(c));
      RuleFor(e => e.Close).SetValidator((c, p) => new ClosePriceValidator(c));
      RuleFor(e => e.Volume).SetValidator(new VolumeValidator());
    }
  }


  public class VolumeValidator : AbstractValidator<decimal>
  {
    public VolumeValidator()
    {
      RuleFor(e => e).GreaterThanOrEqualTo(0).WithMessage("Must be >= 0");
    }
  }

  public class OpenPriceValidator : SidePriceValidator
  {
    public OpenPriceValidator(IPayload candle) : this(candle.High, candle.Low) { }
    public OpenPriceValidator(decimal high, decimal low) : base(high, low)
    {

    }
  }

  public class HighPriceValidator : PriceValidator
  {
    public HighPriceValidator(IPayload candle) : this(candle.Open, candle.Low, candle.Close) { }
    public HighPriceValidator(decimal open, decimal low, decimal close) : base()
    {
      RuleFor(e => e).GreaterThanOrEqualTo(open).WithMessage("Must be greater or equal Open price");
      RuleFor(e => e).GreaterThanOrEqualTo(low).WithMessage("Must be greater or equal Low price");
      RuleFor(e => e).GreaterThanOrEqualTo(close).WithMessage("Must be greater or equal Close price");
    }
  }

  public class LowPriceValidator : PriceValidator
  {
    public LowPriceValidator(IPayload candle) : this(candle.Open, candle.High, candle.Close) { }
    public LowPriceValidator(decimal open, decimal high, decimal close) : base()
    {
      RuleFor(e => e).LessThanOrEqualTo(open).WithMessage("Must be less or equal Open price");
      RuleFor(e => e).LessThanOrEqualTo(high).WithMessage("Must be less or equal High price");
      RuleFor(e => e).LessThanOrEqualTo(close).WithMessage("Must be less or equal Close price");
    }
  }
  public class ClosePriceValidator : SidePriceValidator
  {
    public ClosePriceValidator(IPayload candle) : this(candle.High, candle.Low) { }
    public ClosePriceValidator(decimal high, decimal low) : base(high, low)
    {

    }
  }
}

public class SidePriceValidator : PriceValidator
{
  public SidePriceValidator(decimal high, decimal low) : base()
  {
    RuleFor(e => e).LessThanOrEqualTo(high).WithMessage("Must be less or equal High price");
    RuleFor(e => e).GreaterThanOrEqualTo(low).WithMessage("Must be greater or equal Close price");
  }
}

public class PriceValidator : AbstractValidator<decimal>
{
  public PriceValidator()
  {
    RuleFor(e => e).GreaterThanOrEqualTo(0).WithMessage("Must be >= 0");
  }
}
