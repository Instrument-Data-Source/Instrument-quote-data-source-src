using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
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
      Seconds = ToSeconds(value);
    }
  }

  public static int ToSeconds(TimeFrame.Enum enumId)
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
  public static TimeFrame.Enum GetEnumFrom(int id)
  {
    return (TimeFrame.Enum)Enum.ToObject(typeof(TimeFrame.Enum), id);
  }
  private readonly List<Chart> _charts = new();
  public virtual IEnumerable<Chart> Charts => _charts.AsReadOnly();
  private readonly List<LoadedPeriod> _LoadedPeriods = new();
  public virtual IEnumerable<LoadedPeriod> LoadedPeriods => _LoadedPeriods.AsReadOnly();
  private readonly List<CandleAggregate.Model.Candle> _candles = new();
  public virtual IEnumerable<CandleAggregate.Model.Candle> Candles => _candles.AsReadOnly();
}

public static class TimeFrameMapper
{

  public static TimeFrame ToEntity(this TimeFrame.Enum enumValue)
  {
    return new TimeFrame(enumValue);
  }
}