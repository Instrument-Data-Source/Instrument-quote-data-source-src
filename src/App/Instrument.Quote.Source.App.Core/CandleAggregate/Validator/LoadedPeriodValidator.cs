using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
public class LoadedPeriodValidator : AbstractValidator<LoadedPeriod>
{
  public LoadedPeriodValidator()
  {
    RuleLevelCascadeMode = CascadeMode.Continue;
    RuleFor(e => e).SetValidator(e => new DatesLoadedPeriodValidator()).WithMessage("Dates in LoadedPeriod is Invalid");
  }
}
public class DatesLoadedPeriodValidator : AbstractValidator<LoadedPeriod>
{
  public DatesLoadedPeriodValidator()
  {
    RuleLevelCascadeMode = CascadeMode.Continue;
    RuleFor(e => e.FromDate).Must(e => e.Kind == DateTimeKind.Utc).WithMessage("From DateTime must be in UTC format.");
    RuleFor(e => e.UntillDate).Must(e => e.Kind == DateTimeKind.Utc).WithMessage("Untill DateTime must be in UTC format.");
    RuleFor(e => e.FromDate).LessThan(e => e.UntillDate).WithMessage("From DateTime must be LT Untill DateTime.");
  }
}
public class NewFromDateLoadedPeriodValidator : AbstractValidator<DateTime>
{
  public NewFromDateLoadedPeriodValidator(LoadedPeriod loadedPeriod)
  {
    RuleLevelCascadeMode = CascadeMode.Continue;
    RuleFor(e => e).Must(e => e.Kind == DateTimeKind.Utc).WithMessage("From DateTime must be in UTC format.");
    RuleFor(e => e).LessThan(loadedPeriod.UntillDate).WithMessage("From DateTime must be LT Untill DateTime.");
    RuleFor(e => e).LessThanOrEqualTo(loadedPeriod.FromDate).WithMessage("New From DateTime must be LE From DateTime.");
  }
}

public class NewUntillDateLoadedPeriodValidator : AbstractValidator<DateTime>
{
  public NewUntillDateLoadedPeriodValidator(LoadedPeriod loadedPeriod)
  {
    RuleLevelCascadeMode = CascadeMode.Continue;
    RuleFor(e => e).Must(e => e.Kind == DateTimeKind.Utc).WithMessage("Untill DateTime must be in UTC format.");
    RuleFor(e => e).GreaterThan(loadedPeriod.FromDate).WithMessage("Untill DateTime must be GT From DateTime.");
    RuleFor(e => e).GreaterThanOrEqualTo(loadedPeriod.UntillDate).WithMessage("New Untill DateTime must be GE Untill DateTime.");
  }
}

public class ExtensionPeriodLoadedPeriodValidator : AbstractValidator<LoadedPeriod>
{
  public ExtensionPeriodLoadedPeriodValidator(LoadedPeriod existLoadedPeriod)
  {
    RuleLevelCascadeMode = CascadeMode.Continue;
    RuleFor(e => e).Must(e => e.FromDate == existLoadedPeriod.UntillDate || e.UntillDate == existLoadedPeriod.FromDate)
        .WithMessage("Candle Extensions allows only using connected periods");
    RuleFor(e => e.InstrumentId).Equal(existLoadedPeriod.InstrumentId)
        .WithMessage("Extension period must have same Instrument Id");
    RuleFor(e => e.TimeFrameId).Equal(existLoadedPeriod.TimeFrameId)
        .WithMessage("Extension period must have same TimeFrame Id");
  }
}
