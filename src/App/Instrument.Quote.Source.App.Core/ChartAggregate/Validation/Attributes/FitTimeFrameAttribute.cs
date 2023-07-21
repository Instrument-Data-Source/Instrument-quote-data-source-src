using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;

/// <summary>
/// Validator to check if DateTime valid to TimeFrame
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class FitTimeFrameAttribute : absDependableOfPropAttribute
{
  public FitTimeFrameAttribute(string TimeFrameIdPropName) : base(TimeFrameIdPropName, null) { }

  public FitTimeFrameAttribute(string TimeFrameIdPropName, bool useContextItems) : base(TimeFrameIdPropName, useContextItems)
  {
  }
  public override bool RequiresValidationContext => true;
  protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
  {
    var tfId = getId(validationContext);
    var timeFrameEnum = (TimeFrame.Enum)Enum.ToObject(typeof(TimeFrame.Enum), tfId);
    var dt = (DateTime)value!;

    if (!IsValid(dt, timeFrameEnum, out var validationMessage))
      return new ValidationResult(validationMessage, new string[] { validationContext.MemberName! });

    return ValidationResult.Success;
  }

  public static bool IsValid(DateTime dateTime, TimeFrame.Enum timeFrameEnum, out string validationMessage)
  {
    validationMessage = "";
    switch (timeFrameEnum)
    {
      case TimeFrame.Enum.m1:
        return isValidateMinuteTf(dateTime, 1, out validationMessage);
      case TimeFrame.Enum.m5:
        return isValidateMinuteTf(dateTime, 5, out validationMessage);
      case TimeFrame.Enum.m10:
        return isValidateMinuteTf(dateTime, 10, out validationMessage);
      case TimeFrame.Enum.m15:
        return isValidateMinuteTf(dateTime, 15, out validationMessage);
      case TimeFrame.Enum.m30:
        return isValidateMinuteTf(dateTime, 30, out validationMessage);
      case TimeFrame.Enum.H1:
        return isValidateHourTf(dateTime, 1, out validationMessage);
      case TimeFrame.Enum.H4:
        return isValidateHourTf(dateTime, 4, out validationMessage);
      case TimeFrame.Enum.D1:
        return isValidateDayTf(dateTime, out validationMessage);
      case TimeFrame.Enum.W1:
        return isValidateWeekTf(dateTime, out validationMessage);
      case TimeFrame.Enum.M:
        return isValidateMonthTf(dateTime, out validationMessage);
      default:
        throw new NotImplementedException("Unexpected TimeFrame Time");
    }
  }

  public static bool ValidateCandles(IEnumerable<DateTime> dateTimes, TimeFrame.Enum timeFrameEnum, out string[] validationMessages)
  {
    var msgArr = new List<string>();
    foreach (var dt in dateTimes)
    {
      if (!IsValid(dt, timeFrameEnum, out var msg))
        msgArr.Add(msg);
    }
    validationMessages = msgArr.ToArray();
    return msgArr.Count == 0;
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