using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.Validation;
using Instrument.Quote.Source.App.Core.Validation.Wrapper;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Model;

public partial class Chart : EntityBaseValidation
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
  private int _TimeFrameId;
  /// <summary>
  /// Id of <see cref="TimeFrame"/> realated to entity
  /// </summary>
  /// <value></value>
  public int TimeFrameId
  {
    get => _TimeFrameId;
    private set
    {
      _TimeFrame = null;
      _TimeFrameId = value;
    }
  }
  private TimeFrame _TimeFrame;
  /// <summary>
  /// <see cref="TimeFrame"/> realated to entity
  /// </summary>
  /// <value></value>  
  public virtual TimeFrame TimeFrame
  {
    get => _TimeFrame;
    private set
    {
      TimeFrameId = value.Id;
      _TimeFrame = value;
    }
  }

  #endregion

  #region Instrument relation
  private int _InstrumentId;
  /// <summary>
  /// Id of <see cref="Instrument"/> realated to entity
  /// </summary>
  /// <value></value>
  public int InstrumentId
  {
    get => _InstrumentId; private set
    {
      _Instrument = null;
      _InstrumentId = value;
    }
  }
  private ent.Instrument _Instrument;
  /// <summary>
  /// <see cref="Instrument"/> realated to entity
  /// </summary>
  /// <value></value>  
  public virtual ent.Instrument Instrument
  {
    get => _Instrument;
    private set
    {
      InstrumentId = value.Id;
      _Instrument = value;
    }
  }
  #endregion

  #region Candles relation
  private List<Candle> _candles;

  public virtual IEnumerable<Candle>? Candles => _candles != null ? _candles.AsReadOnly() : null;
  #endregion

}