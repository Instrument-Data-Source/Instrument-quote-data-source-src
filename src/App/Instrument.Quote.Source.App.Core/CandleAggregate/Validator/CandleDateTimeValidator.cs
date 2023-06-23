using FluentValidation;
using FluentValidation.Results;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;

public class CandleDateTimeValidator : AbstractValidator<DateTime>
{

  public CandleDateTimeValidator(TimeFrame.Enum timeFrame)
  {
    var rule_builder = RuleFor(e => e);
    switch (timeFrame)
    {
      case TimeFrame.Enum.m1:
        rule_builder.ValidateMinuteTf(1);
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
        rule_builder.ValidateHourTf(1);
        break;
      case TimeFrame.Enum.H4:
        rule_builder.ValidateHourTf(4);
        break;
      case TimeFrame.Enum.D1:
        rule_builder.ValidateDayTf();
        break;
      case TimeFrame.Enum.W1:
        rule_builder.ValidateDayTf();
        break;
      case TimeFrame.Enum.M:
        rule_builder.ValidateMonthTf();
        break;
      default:
        throw new NotImplementedException("Unexpected TimeFrame Time");
    }
  }
  public CandleDateTimeValidator()
  {
    var rule_builder = RuleFor(e => e).Must(e => e.Kind == DateTimeKind.Utc).WithMessage("DateTime must be in UTC format.");
    rule_builder.Custom((dt, context) =>
    {
      if (context.RootContextData.TryGetValue("timeframe", out var tf))
      {
        TimeFrame.Enum timeFrameId = (TimeFrame.Enum)Enum.ToObject(typeof(TimeFrame.Enum), tf);
        switch (timeFrameId)
        {
          case TimeFrame.Enum.m1:
            dt.ValidateMinuteTf(1, context);
            break;
          case TimeFrame.Enum.m5:
            dt.ValidateMinuteTf(5, context);
            break;
          case TimeFrame.Enum.m10:
            dt.ValidateMinuteTf(10, context);
            break;
          case TimeFrame.Enum.m15:
            dt.ValidateMinuteTf(15, context);
            break;
          case TimeFrame.Enum.m30:
            dt.ValidateMinuteTf(30, context);
            break;
          case TimeFrame.Enum.H1:
            dt.ValidateHourTf(1, context);
            break;
          case TimeFrame.Enum.H4:
            dt.ValidateHourTf(4, context);
            break;
          case TimeFrame.Enum.D1:
            dt.ValidateDayTf(context);
            break;
          case TimeFrame.Enum.W1:
            dt.ValidateWeekTf(context);
            break;
          case TimeFrame.Enum.M:
            dt.ValidateMonthTf(context);
            break;
          default:
            throw new NotImplementedException("Unexpected TimeFrame Time");
        }
      }
    });
  }
}

public static class DateTimeValidatorRules
{
  public static void ValidateMinuteTf(this IRuleBuilder<DateTime, DateTime> Rule, int interval)
  {
    Rule.Must((dt, val, context) => dt.ValidateMinuteTf(interval, context));
  }

  public static bool ValidateMinuteTf(this DateTime dt, int interval, ValidationContext<DateTime> context)
  {
    var isValid = true;
    if (!Validate_No_Sec(dt))
    {
      context.AddFailure($"DateTime in TimeFrame m{interval} must have zero seconds, microseconds, miliseconds, nanoseconds.");
      isValid = false;
    }
    if (!Validate_Is_Xmin(dt, interval))
    {
      context.AddFailure($"DateTime in TimeFrame m{interval} must have {interval} minute interval.");
      isValid = false;
    }
    return isValid;
  }
  public static void ValidateHourTf(this IRuleBuilder<DateTime, DateTime> Rule, int interval)
  {
    Rule.Must((dt, val, context) => dt.ValidateHourTf(interval, context));
  }

  public static bool ValidateHourTf(this DateTime dt, int interval, ValidationContext<DateTime> context)
  {
    var isValid = true;
    if (!Validate_No_Min(dt))
    {
      context.AddFailure($"DateTime in TimeFrame H{interval} must have zero minuts, seconds, microseconds, miliseconds, nanoseconds.");
      isValid = false;
    }
    if (!Validate_Is_Xhour(dt, interval))
    {
      context.AddFailure($"DateTime in TimeFrame H{interval} must have {interval} hour interval.");
      isValid = false;
    }
    return isValid;
  }
  public static void ValidateDayTf(this IRuleBuilder<DateTime, DateTime> Rule)
  {
    Rule.Must((dt, val, context) => dt.ValidateDayTf(context));
  }

  public static bool ValidateDayTf(this DateTime dt, ValidationContext<DateTime> context)
  {
    var isValid = true;
    if (!Validate_No_Hour(dt))
    {
      context.AddFailure("DateTime in TimeFrame D1 must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.");
      isValid = false;
    }
    return isValid;
  }
  public static void ValidateWeekTf(this IRuleBuilder<DateTime, DateTime> Rule)
  {
    Rule.Must((dt, val, context) => dt.ValidateWeekTf(context));
  }
  public static bool ValidateWeekTf(this DateTime dt, ValidationContext<DateTime> context)
  {
    var isValid = true;
    if (!Validate_No_Hour(dt))
    {
      context.AddFailure("DateTime in TimeFrame W1 must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.");
      isValid = false;
    }
    if (!Validate_Is_Monday(dt))
    {
      context.AddFailure("DateTime in TimeFrame W1 must be Monday.");
      isValid = false;
    }
    return isValid;
  }
  public static void ValidateMonthTf(this IRuleBuilder<DateTime, DateTime> Rule)
  {
    Rule.Must((dt, val, context) => dt.ValidateMonthTf(context));
  }
  public static bool ValidateMonthTf(this DateTime dt, ValidationContext<DateTime> context)
  {
    var isValid = true;
    if (!Validate_No_Hour(dt))
    {
      context.AddFailure("DateTime in TimeFrame M must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.");
      isValid = false;
    }
    if (!Validate_Is_1st_day(dt))
    {
      context.AddFailure("DateTime in TimeFrame M must be 1st day.");
      isValid = false;
    }
    return isValid;
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

  public static bool Validate_Is_1st_day(DateTime dt)
  {
    return dt.Day == 1;
  }
}