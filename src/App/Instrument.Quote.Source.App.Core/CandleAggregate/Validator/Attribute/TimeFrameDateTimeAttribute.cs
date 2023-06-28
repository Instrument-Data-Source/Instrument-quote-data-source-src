using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class TimeFrameDateTimeAttribute : ValidationAttribute
{
  public override bool RequiresValidationContext => true;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var loadedPeriod = (LoadedPeriod)validationContext.ObjectInstance;
    var timeFrameEnum = (TimeFrame.Enum)Enum.ToObject(typeof(TimeFrame.Enum), loadedPeriod.TimeFrameId);
    var candles = (IEnumerable<Candle>)value!;

    if (!IsValid(candles.Select(c => c.DateTime), timeFrameEnum, out var validationMessage))
      return new ValidationResult(validationMessage, new string[] { validationContext.MemberName! });

    return ValidationResult.Success;
  }

  public bool IsValid(IEnumerable<DateTime> dateTimes, TimeFrame.Enum timeFrameEnum, out string validationMessage)
  {
    var returnMsg = "";
    var isValid = true;
    switch (timeFrameEnum)
    {
      case TimeFrame.Enum.m1:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateMinuteTf(dt, 1, out returnMsg));
        break;
      case TimeFrame.Enum.m5:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateMinuteTf(dt, 5, out returnMsg));
        break;
      case TimeFrame.Enum.m10:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateMinuteTf(dt, 10, out returnMsg));
        break;
      case TimeFrame.Enum.m15:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateMinuteTf(dt, 15, out returnMsg));
        break;
      case TimeFrame.Enum.m30:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateMinuteTf(dt, 30, out returnMsg));
        break;
      case TimeFrame.Enum.H1:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateHourTf(dt, 1, out returnMsg));
        break;
      case TimeFrame.Enum.H4:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateHourTf(dt, 4, out returnMsg));
        break;
      case TimeFrame.Enum.D1:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateDayTf(dt, out returnMsg));
        break;
      case TimeFrame.Enum.W1:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateWeekTf(dt, out returnMsg));
        break;
      case TimeFrame.Enum.M:
        isValid = ValidateCandles(dateTimes, (dt) => isValidateMonthTf(dt, out returnMsg));
        break;
      default:
        throw new NotImplementedException("Unexpected TimeFrame Time");
    }
    validationMessage = returnMsg;
    return isValid;
  }

  public static bool ValidateCandles(IEnumerable<DateTime> dateTimes, Func<DateTime, bool> validateFunc)
  {
    foreach (var dt in dateTimes)
    {
      if (!validateFunc(dt))
        return false;
    }
    return true;
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