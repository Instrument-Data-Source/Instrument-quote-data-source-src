using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.Validation;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
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

  public bool IsLast { get; set; }
  public bool IsFullCalc { get; set; }
  #region Chart relation

  private int _JoinedChartId;
  public int JoinedChartId
  {
    get => _JoinedChartId;
    private set
    {
      _JoinedChart = null;
      _JoinedChartId = value;
    }
  }

  private JoinedChart _JoinedChart;
  public virtual JoinedChart JoinedChart
  {
    get => _JoinedChart; private set
    {
      _JoinedChartId = value.Id;
      _JoinedChart = value;
    }
  }


  #endregion
}