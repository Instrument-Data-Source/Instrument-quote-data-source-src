using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Core.Validation;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;


public partial class JoinedChart : EntityBaseValidation
{
  [Required]
  [UTCKind]
  [CompareTo(CompType.LT, nameof(UntillDate))]
  public DateTime FromDate { get; private set; }

  [Required]
  [UTCKind]
  [CompareTo(CompType.GT, nameof(FromDate))]
  public DateTime UntillDate { get; private set; }

  #region TimeFrame relation
  private int _targetTimeFrameId;
  /// <summary>
  /// Id of <see cref="TimeFrameAggregate.Model.TimeFrame"/> realated to entity
  /// </summary>
  /// <value></value>
  public int TargetTimeFrameId
  {
    get => _targetTimeFrameId;
    private set
    {
      _targetTimeFrame = null;
      _targetTimeFrameId = value;
    }
  }
  private TimeFrame _targetTimeFrame;
  /// <summary>
  /// <see cref="TimeFrameAggregate.Model.TimeFrame"/> realated to entity
  /// </summary>
  /// <value></value>  
  public virtual TimeFrame TargetTimeFrame
  {
    get => _targetTimeFrame;
    private set
    {
      TargetTimeFrameId = value.Id;
      _targetTimeFrame = value;
    }
  }

  #endregion

  #region Base Chart relation
  private int _stepChartId;
  /// <summary>
  /// Id of <see cref="ChartAggregate.Model.Chart"/> realated to entity
  /// </summary>
  /// <value></value>

  public int StepChartId
  {
    get => _stepChartId;
    private set
    {
      _stepChart = null;
      _stepChartId = value;
    }
  }

  private Chart _stepChart;
  /// <summary>
  /// <see cref="StepChart"/> realated to entity
  /// </summary>
  /// <value></value>  
  public Chart StepChart
  {
    get => _stepChart;
    private set
    {
      StepChartId = value.Id;
      _stepChart = value;
    }
  }
  #endregion


  #region Candles relation
  private List<JoinedCandle> _joinedCandles;

  public virtual IEnumerable<JoinedCandle>? JoinedCandles => _joinedCandles != null ? _joinedCandles.AsReadOnly() : null;
  #endregion

}