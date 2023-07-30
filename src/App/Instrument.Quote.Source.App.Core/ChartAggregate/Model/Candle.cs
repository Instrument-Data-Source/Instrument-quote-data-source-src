using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.ChartAggregate.Validation;
using Instrument.Quote.Source.App.Core.Validation;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;
public partial class Candle : EntityBaseValidation
{
  [Required]
  [UTCKind]
  public DateTime DateTime { get; private set; }

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