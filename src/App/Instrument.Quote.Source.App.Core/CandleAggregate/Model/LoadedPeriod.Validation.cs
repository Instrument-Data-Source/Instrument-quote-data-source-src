using FluentValidation;
using FluentValidation.Internal;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;

public partial class LoadedPeriod
{
  protected override FluentValidation.Results.ValidationResult ValidateSelf(Action<ValidationStrategy<LoadedPeriod>> options) =>
  new Validator().Validate(this, options);

  public class Validator : AbstractValidator<LoadedPeriod>
  {
    public Validator()
    {
      RuleFor(e => e).SetValidator(new PayloadValidator());
    }
  }

  public class PayloadValidator : AbstractValidator<LoadedPeriod.IPayload>
  {
    public PayloadValidator()
    {
      RuleFor(e => e.FromDate).SetValidator((lp) => new FromDateValidator(lp));
      RuleFor(e => e.UntillDate).SetValidator((lp) => new UntillDateValidator(lp));
    }
  }


  /// <summary>
  /// Validator of candle enumerable for loaded period
  /// </summary>
  public class CandleArrForPeriodValidator : AbstractValidator<IEnumerable<Candle>>
  {
    public CandleArrForPeriodValidator(LoadedPeriod.IPayload loadedPeriod)
    {
      RuleForEach(e => e).SetValidator(new CandleForPeriodValidator(loadedPeriod));
    }
  }

  /// <summary>
  /// Validator of candle for loaded period
  /// </summary>
  public class CandleForPeriodValidator : AbstractValidator<Candle>
  {
    public CandleForPeriodValidator(LoadedPeriod.IPayload loadedPeriod)
    {
      RuleFor(e => e).SetValidator(new CandlePayloadForPeriodValidator(loadedPeriod));
      RuleFor(e => e.InstrumentId).Equal(loadedPeriod.InstrumentId).WithMessage("Instument Id in candle must be == Instrument id in period");
      RuleFor(e => e.TimeFrameId).Equal(loadedPeriod.TimeFrameId).WithMessage("TimeFrame Id in candle must be == Timeframe id in period");
    }
  }

  /// <summary>
  /// Validator of candle payload for loaded period
  /// </summary>
  public class CandlePayloadForPeriodValidator : Candle.CandlePayloadValidator
  {
    public CandlePayloadForPeriodValidator(LoadedPeriod.IPayload loadedPeriod)
    {
      RuleFor(e => e.DateTime).SetValidator(new DateTimeForTimeFrameValidator(TimeFrame.GetEnumFrom(loadedPeriod.TimeFrameId)));
      RuleFor(e => e.DateTime).GreaterThanOrEqualTo(loadedPeriod.FromDate).WithMessage("Candle date must be >= from date");
      RuleFor(e => e.DateTime).LessThan(loadedPeriod.UntillDate).WithMessage("Candle date must be  < untill date");
    }
  }

  public class ExtensionPeriodLoadedPeriodValidator : AbstractValidator<LoadedPeriod>
  {
    public ExtensionPeriodLoadedPeriodValidator(LoadedPeriod existLoadedPeriod)
    {
      RuleFor(e => e).Must(e => e.FromDate == existLoadedPeriod.UntillDate || e.UntillDate == existLoadedPeriod.FromDate)
          .WithMessage("Candle Extensions allows only using connected periods");
      RuleFor(e => e.InstrumentId).Equal(existLoadedPeriod.InstrumentId)
          .WithMessage("Extension period must have same Instrument Id");
      RuleFor(e => e.TimeFrameId).Equal(existLoadedPeriod.TimeFrameId)
          .WithMessage("Extension period must have same TimeFrame Id");
    }
  }

  public class FromDateValidator : AbstractValidator<DateTime>
  {
    public FromDateValidator(LoadedPeriod.IPayload loadedPeriod) : this(loadedPeriod.UntillDate)
    {

    }
    public FromDateValidator(DateTime untillDateTime)
    {
      RuleFor(e => e).SetValidator(new DateTimeValidator());
      RuleFor(e => e).LessThan(untillDateTime).WithMessage("From DateTime must be < Untill DateTime.");
    }
  }

  public class UntillDateValidator : AbstractValidator<DateTime>
  {
    public UntillDateValidator(LoadedPeriod.IPayload loadedPeriod) : this(loadedPeriod.FromDate)
    {

    }
    public UntillDateValidator(DateTime fromDateTime)
    {
      {
        RuleFor(e => e).SetValidator(new DateTimeValidator());
        RuleFor(e => e).GreaterThan(fromDateTime).WithMessage("Untill DateTime must be GT From DateTime.");
      }
    }
  }
}

