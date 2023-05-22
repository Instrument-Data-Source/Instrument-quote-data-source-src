using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;
public partial class LoadedPeriod : EntityBase
{
  public LoadedPeriod(int instrumentId,
                      int timeFrameId,
                      DateTime fromDate,
                      DateTime untillDate)
  {
    InstrumentId = instrumentId;
    TimeFrameId = timeFrameId;
    _fromDate = fromDate;
    _untillDate = untillDate;
    new LoadedPeriodValidator().ValidateAndThrow(this);
  }
  private DateTime _fromDate;
  [Required]
  public DateTime FromDate
  {
    get => _fromDate;
    private set
    {
      new NewFromDateLoadedPeriodValidator(this).ValidateAndThrow(value);
      _fromDate = value;
    }
  }
  private DateTime _untillDate;
  [Required]
  public DateTime UntillDate
  {
    get => _untillDate;
    private set
    {
      new NewUntillDateLoadedPeriodValidator(this).ValidateAndThrow(value);
      _untillDate = value;
    }
  }

  #region TimeFrame relation
  /// <summary>
  /// Id of <see cref="TimeFrame"/> realated to entity
  /// </summary>
  /// <value></value>

  public int TimeFrameId { get; private set; }

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
  /// <summary>
  /// Id of <see cref="Instrument"/> realated to entity
  /// </summary>
  /// <value></value>

  public int InstrumentId { get; private set; }

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
  public virtual IEnumerable<Candle> Candles => _candles.AsReadOnly();

  private LoadedPeriod AddCandles(IEnumerable<Candle> candles)
  {
    new CandlesForLoadedPeriodValidator(this).ValidateAndThrow(candles);

    _candles.AddRange(candles);
    return this;
  }
  #endregion

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

}