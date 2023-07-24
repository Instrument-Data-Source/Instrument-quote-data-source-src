using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation.Attributes;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;
public partial class JoinedCandle : EntityBaseValidation
{
  [Required]
  [UTCKind]
  public DateTime StepDateTime { get; private set; }
  [Required]
  [UTCKind]
  public DateTime TargetDateTime { get; private set; }

  [Required]
  [CompareTo(CompType.GE, nameof(Low))]
  [CompareTo(CompType.LE, nameof(High))]
  public int Open { get; private set; }

  [Required]
  [CompareTo(CompType.GE, nameof(Low))]
  [CompareTo(CompType.LE, nameof(High))]
  public int Close { get; private set; }

  [Required]
  [CompareTo(CompType.GE, nameof(Open), nameof(Low), nameof(Close))]
  public int High { get; private set; }

  [Required]
  [CompareTo(CompType.LE, nameof(Open), nameof(High), nameof(Close))]
  public int Low { get; private set; }

  [Required]
  [Range(0, int.MaxValue)]
  public int Volume { get; private set; }

  public bool IsLast { get; private set; }

  #region TimeFrame relation
  private int _TargetTimeFrameId;
  /// <summary>
  /// Id of <see cref="TimeFrame"/> realated to entity
  /// </summary>
  /// <value></value>
  public int TargetTimeFrameId
  {
    get => _TargetTimeFrameId;
    private set
    {
      _TargetTimeFrame = null;
      _TargetTimeFrameId = value;
    }
  }
  private TimeFrame _TargetTimeFrame;
  /// <summary>
  /// <see cref="TimeFrame"/> realated to entity
  /// </summary>
  /// <value></value>  
  public virtual TimeFrame TargetTimeFrame
  {
    get => _TargetTimeFrame;
    private set
    {
      TargetTimeFrameId = value.Id;
      _TargetTimeFrame = value;
    }
  }

  #endregion

  #region Chart relation

  private int _ChartId;
  public int ChartId
  {
    get => _ChartId;
    private set
    {
      _Chart = null;
      _ChartId = value;
    }
  }

  private Chart _Chart;
  public virtual Chart Chart
  {
    get => _Chart; private set
    {
      _ChartId = value.Id;
      _Chart = value;
    }
  }


  #endregion
}