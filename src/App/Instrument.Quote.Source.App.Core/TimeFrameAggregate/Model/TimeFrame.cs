using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;

public class TimeFrame : EnumEntity<TimeFrame.Enum>
{
  public new const int NameLenght = 3;
  public enum Enum
  {
    M = 1,
    W1 = 2,
    D1 = 3,
    H4 = 4,
    H1 = 5,
    m30 = 6,
    m15 = 7,
    m10 = 8,
    m5 = 9,
    m1 = 10

  }
  /// <summary>
  /// Second in TimeFrame
  /// </summary>
  /// <value></value>
  [Required]
  public int Seconds { get; private set; }

  public TimeFrame(int id) : base(id)
  {
  }

  public TimeFrame(Enum Id) : base(Id)
  {
  }

  public override TimeFrame.Enum EnumId
  {
    get => base.EnumId;
    set
    {
      base.EnumId = value;
      Seconds = value.ToSeconds();
    }
  }


  public static TimeFrame.Enum GetEnumFrom(int id)
  {
    return (TimeFrame.Enum)Enum.ToObject(typeof(TimeFrame.Enum), id);
  }
  #region  Chart relation
  private readonly List<Chart> _charts;
  public virtual IEnumerable<Chart>? Charts => _charts != null ? _charts.AsReadOnly() : null;
  #endregion

  #region Joined Chart relation
  private readonly List<JoinedChart> _joinedCharts;
  public virtual IEnumerable<JoinedChart>? JoinedCharts => _joinedCharts != null ? _joinedCharts.AsReadOnly() : null;
  #endregion
}

public static class TimeFrameMapper
{

  public static TimeFrame ToEntity(this TimeFrame.Enum enumValue)
  {
    return new TimeFrame(enumValue);
  }
  public static DateTime GetUntillDateTimeFor(this TimeFrame.Enum enumValue, DateTime dateTime)
  {
    DateTime _retDt = enumValue.GetFromDateTimeFor(dateTime);
    switch (enumValue)
    {
      case TimeFrame.Enum.M:
        _retDt = _retDt.AddMonths(1);
        break;
      case TimeFrame.Enum.W1:
        _retDt = _retDt.AddDays(7);
        break;
      case TimeFrame.Enum.D1:
        _retDt = _retDt.AddDays(1);
        break;
      case TimeFrame.Enum.H4:
        _retDt = _retDt.AddHours(4);
        break;
      case TimeFrame.Enum.H1:
        _retDt = _retDt.AddHours(1);
        break;
      case TimeFrame.Enum.m30:
        _retDt = _retDt.AddMinutes(30);
        break;
      case TimeFrame.Enum.m15:
        _retDt = _retDt.AddMinutes(15);
        break;
      case TimeFrame.Enum.m10:
        _retDt = _retDt.AddMinutes(10);
        break;
      case TimeFrame.Enum.m5:
        _retDt = _retDt.AddMinutes(5);
        break;
      case TimeFrame.Enum.m1:
        _retDt = _retDt.AddMinutes(1);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, $"Unexpected value of {nameof(TimeFrame.Enum)}");
    }

    return _retDt;
  }
  public static DateTime GetFromDateTimeFor(this TimeFrame.Enum enumValue, DateTime dateTime)
  {
    DateTime _retDt;
    switch (enumValue)
    {
      case TimeFrame.Enum.M:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, 1);
        break;
      case TimeFrame.Enum.W1:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day) - new TimeSpan(CalcSubstractDaysToGetMonday(dateTime.DayOfWeek), 0, 0, 0);
        break;
      case TimeFrame.Enum.D1:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        break;
      case TimeFrame.Enum.H4:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour.GetFirstOf(4), 0, 0);
        break;
      case TimeFrame.Enum.H1:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, 0, 0);
        break;
      case TimeFrame.Enum.m30:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute.GetFirstOf(30), 0);
        break;
      case TimeFrame.Enum.m15:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute.GetFirstOf(15), 0);
        break;
      case TimeFrame.Enum.m10:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute.GetFirstOf(10), 0);
        break;
      case TimeFrame.Enum.m5:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute.GetFirstOf(5), 0);
        break;
      case TimeFrame.Enum.m1:
        _retDt = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(enumValue), enumValue, $"Unexpected value of {nameof(TimeFrame.Enum)}");
    }

    return _retDt.ToUniversalTime();
  }

  private static int GetFirstOf(this int value, int groupSize)
  {
    return value / groupSize * groupSize;
  }

  public static int CalcSubstractDaysToGetMonday(DayOfWeek dayOfWeek)
  {
    return (((int)dayOfWeek - 1) + 7) % 7;
  }

  public static int ToSeconds(this TimeFrame.Enum enumId)
  {

    switch (enumId)
    {
      case TimeFrame.Enum.M:
        return (365 / 12) * 24 * 60 * 60;
      case TimeFrame.Enum.W1:
        return 7 * 24 * 60 * 60;
      case TimeFrame.Enum.D1:
        return 24 * 60 * 60;
      case TimeFrame.Enum.H4:
        return 60 * 60 * 4;
      case TimeFrame.Enum.H1:
        return 60 * 60;
      case TimeFrame.Enum.m30:
        return 60 * 30;
      case TimeFrame.Enum.m15:
        return 60 * 15;
      case TimeFrame.Enum.m10:
        return 60 * 10;
      case TimeFrame.Enum.m5:
        return 60 * 5;
      case TimeFrame.Enum.m1:
        return 60 * 1;
      default:
        throw new ArgumentOutOfRangeException(nameof(enumId), enumId, "Unexpected type of TimeFrame");
    }
  }
}