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
  /// Id of <see cref="TimeFrame"/> realated to entity
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
  /// <see cref="TimeFrame"/> realated to entity
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
  private int _baseChartId;
  /// <summary>
  /// Id of <see cref="Chart"/> realated to entity
  /// </summary>
  /// <value></value>

  public int BaseChartId
  {
    get => _baseChartId;
    private set
    {
      _baseChart= null;
      _baseChartId = value;
    }
  }

  private Chart _baseChart;
  /// <summary>
  /// <see cref="BaseChart"/> realated to entity
  /// </summary>
  /// <value></value>  
  public Chart BaseChart
  {
    get => _baseChart;
    private set
    {
      BaseChartId = value.Id;
      _baseChart = value;
    }
  }
  #endregion

  #region Candles relation
  private readonly List<JoinedCandle> _joinedCandles = new();

  public virtual IEnumerable<JoinedCandle> JoinedCandles => _joinedCandles.AsReadOnly();
  #endregion

}