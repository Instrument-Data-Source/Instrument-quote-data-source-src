using FluentValidation;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
public class DateTimeForTimeFrameValidator : AbstractValidator<DateTime>
{
  public DateTimeForTimeFrameValidator(TimeFrame.Enum timeFrameEnum)
  {
    var returnMsg = "";
    switch (timeFrameEnum)
    {
      case TimeFrame.Enum.m1:
        RuleFor(e => e).Must(dt => isValidateMinuteTf(dt, 1, out var returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.m5:
        RuleFor(e => e).Must(dt => isValidateMinuteTf(dt, 5, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.m10:
        RuleFor(e => e).Must(dt => isValidateMinuteTf(dt, 10, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.m15:
        RuleFor(e => e).Must(dt => isValidateMinuteTf(dt, 15, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.m30:
        RuleFor(e => e).Must(dt => isValidateMinuteTf(dt, 30, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.H1:
        RuleFor(e => e).Must(dt => isValidateHourTf(dt, 1, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.H4:
        RuleFor(e => e).Must(dt => isValidateHourTf(dt, 4, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.D1:
        RuleFor(e => e).Must(dt => isValidateDayTf(dt, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.W1:
        RuleFor(e => e).Must(dt => isValidateWeekTf(dt, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      case TimeFrame.Enum.M:
        RuleFor(e => e).Must(dt => isValidateMonthTf(dt, out returnMsg)).WithMessage((dt) => returnMsg);
        break;
      default:
        throw new NotImplementedException("Unexpected TimeFrame Time");
    }
  }

  public static bool isValidateMinuteTf(DateTime dt, int interval, out string validationMessage)
  {
    if (!Validate_No_Sec(dt))
    {
      validationMessage = $"DateTime in TimeFrame m{interval} must have zero seconds, microseconds, miliseconds, nanoseconds.";
      return false;
    }
    if (!Validate_Is_Xmin(dt, interval))
    {
      validationMessage = $"DateTime in TimeFrame m{interval} must have {interval} minute interval.";
      return false;

    }
    validationMessage = "";
    return true;
  }

  public static bool isValidateHourTf(DateTime dt, int interval, out string validationMessage)
  {
    if (!Validate_No_Min(dt))
    {
      validationMessage =
        $"DateTime in TimeFrame H{interval} must have zero minuts, seconds, microseconds, miliseconds, nanoseconds.";
      return false;
    }
    if (!Validate_Is_Xhour(dt, interval))
    {
      validationMessage =
        $"DateTime in TimeFrame H{interval} must have {interval} hour interval.";
      return false;
    }
    validationMessage = "";
    return true;
  }

  public static bool isValidateDayTf(DateTime dt, out string validationMessage)
  {
    if (!Validate_No_Hour(dt))
    {
      validationMessage =
        "DateTime in TimeFrame D1 must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.";
      return false;
    }
    validationMessage = "";
    return true;
  }

  public static bool isValidateWeekTf(DateTime dt, out string validationMessage)
  {
    if (!Validate_No_Hour(dt))
    {
      validationMessage =
        "DateTime in TimeFrame W1 must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.";
      return false;
    }
    if (!Validate_Is_Monday(dt))
    {
      validationMessage =
        "DateTime in TimeFrame W1 must be Monday.";
      return false;
    }
    validationMessage = "";
    return true;
  }

  public static bool isValidateMonthTf(DateTime dt, out string validationMessage)
  {
    if (!Validate_No_Hour(dt))
    {
      validationMessage =
        "DateTime in TimeFrame M must have zero hour, minute, seconds, microseconds, miliseconds, nanoseconds.";
      return false;
    }
    if (!Validate_Is_1st_day(dt))
    {
      validationMessage =
        "DateTime in TimeFrame M must be 1st day.";
      return false;
    }
    validationMessage = "";
    return true;
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