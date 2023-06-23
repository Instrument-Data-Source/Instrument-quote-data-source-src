using System.ComponentModel.DataAnnotations;
using Ardalis.GuardClauses;
using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;
public partial class LoadedPeriod : EntityBaseExt
{
  [Required]
  [UTCKind]
  [LessThenUntillDate]
  public DateTime FromDate { get; private set; }

  [Required]
  [UTCKind]
  [GreaterThenFromDate]
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

  private readonly List<Candle> _candles = new();
  [CandleInPeriod]
  [NotEmpty]
  [NoDuplicates]
  public virtual IEnumerable<Candle> Candles => _candles.AsReadOnly();
  /*
   private LoadedPeriod AddCandles(IEnumerable<Candle> candles)
   {
     new CandlesForLoadedPeriodValidator(this).ValidateAndThrow(candles);

     _candles.AddRange(candles);
     return this;
   }
   */
  #endregion
  /*
   /// <summary>
   /// Extend current period by new data. Support only connected data. DID NOT support cross data.
   /// </summary>
   /// <param name="newPeriod"></param>
   /// <returns></returns>
   public LoadedPeriod Extend(LoadedPeriod newPeriod)
   {
     new ExtensionPeriodLoadedPeriodValidator(this).ValidateAndThrow(newPeriod);
     FromDate = newPeriod.FromDate < FromDate ? newPeriod.FromDate : FromDate;
     UntillDate = newPeriod.UntillDate > UntillDate ? newPeriod.UntillDate : UntillDate;
     AddCandles(newPeriod.Candles);
     return this;
   }
   */
}