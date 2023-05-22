using FluentValidation;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;

public class CandleDateTimeValidator : AbstractValidator<DateTime>
{
  readonly TimeFrame.Enum timeFrameEnumId;
  public CandleDateTimeValidator(TimeFrame.Enum timeFrameEnumId)
  {
    this.timeFrameEnumId = timeFrameEnumId;
    var rule_builder = RuleFor(e => e).Must(e => e.Kind == DateTimeKind.Utc).WithMessage("DateTime must be in UTC format.");
    switch (this.timeFrameEnumId)
    {
      case TimeFrame.Enum.m1:
        rule_builder.Must(DateTimeValidatorRules.Validate_No_Sec).WithMessage("DateTime in TimeFrame m1 must have zero seconds, microseconds, miliseconds, nanoseconds");
        break;
      case TimeFrame.Enum.m5:
        rule_builder.ValidateMinuteTf(5);
        break;
      case TimeFrame.Enum.m10:
        rule_builder.ValidateMinuteTf(10);
        break;
      case TimeFrame.Enum.m15:
        rule_builder.ValidateMinuteTf(15);
        break;
      case TimeFrame.Enum.m30:
        rule_builder.ValidateMinuteTf(30);
        break;
      case TimeFrame.Enum.H1:
        rule_builder.Must(DateTimeValidatorRules.Validate_No_Min).WithMessage("DateTime in TimeFrame H1 must have zero minute, seconds, microseconds, miliseconds, nanoseconds.");
        break;
      case TimeFrame.Enum.H4:
        rule_builder.ValidateHourTf(4);
        break;
      case TimeFrame.Enum.D1:
        rule_builder.Must(DateTimeValidatorRules.Validate_No_Hour).WithMessage("DateTime in TimeFrame D1 must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.");
        break;
      case TimeFrame.Enum.W1:
        rule_builder.Must(DateTimeValidatorRules.Validate_No_Hour).WithMessage("DateTime in TimeFrame W1 must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.")
                       .Must(DateTimeValidatorRules.Validate_Is_Monday).WithMessage("DateTime in TimeFrame W1 must be Monday.");

        break;
      case TimeFrame.Enum.M:
        rule_builder.Must(DateTimeValidatorRules.Validate_No_Hour).WithMessage("DateTime in TimeFrame M must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.")
                       .Must(e => e.Day == 1).WithMessage("DateTime in TimeFrame M must be 1st day.");

        break;
      default:
        throw new NotImplementedException("Unexpected TimeFrame Time");
    }
  }

}

public static class DateTimeValidatorRules
{
  public static void ValidateMinuteTf(this IRuleBuilder<DateTime, DateTime> Rule, int interval)
  {
    Rule.Must(Validate_No_Sec).WithMessage($"DateTime in TimeFrame m{interval} must have zero seconds, microseconds, miliseconds, nanoseconds.")
        .Must(e => Validate_Is_Xmin(e, interval)).WithMessage($"DateTime in TimeFrame m{interval} must have {interval} minute interval.");
  }
  public static void ValidateHourTf(this IRuleBuilder<DateTime, DateTime> Rule, int interval)
  {
    Rule.Must(Validate_No_Min).WithMessage($"DateTime in TimeFrame H{interval} must have zero minuts, seconds, microseconds, miliseconds, nanoseconds.")
        .Must(e => Validate_Is_Xhour(e, interval)).WithMessage($"DateTime in TimeFrame H{interval} must have {interval} hour interval.");
  }

  public static bool Validate_No_Sec(DateTime dt)
  {
    return dt.Second == 0 && dt.Microsecond == 0 && dt.Millisecond == 0 && dt.Nanosecond == 0;
  }
  public static bool Validate_No_Min(DateTime dt)
  {
    return dt.Minute == 0 && Validate_No_Sec(dt);
  }
  public static bool Validate_No_Hour(DateTime dt)
  {
    return dt.Hour == 0 && Validate_No_Min(dt);
  }

  public static bool Validate_Is_Xmin(DateTime dt, int min)
  {
    return dt.Minute % min == 0;
  }
  public static bool Validate_Is_Xhour(DateTime dt, int hour)
  {
    return dt.Hour % hour == 0;
  }

  public static bool Validate_Is_Monday(DateTime dt)
  {
    return dt.DayOfWeek == DayOfWeek.Monday;
  }
}