using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;

/// <summary>
/// Quotes of <see cref="ent.Instrument"/> 
/// <see cref="ADR-001 Decimal in candle value">ADR-001 Decimal in candle value</see>
/// </summary>
public partial class Candle : EntityBase
{
  public Candle(DateTime dateTime,
                 int openStore,
                 int highStore,
                 int lowStore,
                 int closeStore,
                 int volumeStore,
                 int timeFrameId,
                 int instrumentId)
  {
    DateTime = dateTime;
    OpenStore = openStore;
    HighStore = highStore;
    LowStore = lowStore;
    CloseStore = closeStore;
    VolumeStore = volumeStore;
    TimeFrameId = timeFrameId;
    InstrumentId = instrumentId;
    new CandleValidator((TimeFrame.Enum)timeFrameId).ValidateAndThrow(this);
  }

  [Required]
  public DateTime DateTime { get; private set; }

  public int OpenStore { get; private set; }
  [NotMapped]
  public decimal Open => CalcPriceDecimal(OpenStore);

  public int CloseStore { get; private set; }
  [NotMapped]
  public decimal Close => CalcPriceDecimal(CloseStore);

  public int HighStore { get; private set; }
  [NotMapped]
  public decimal High => CalcPriceDecimal(HighStore);

  public int LowStore { get; private set; }
  [NotMapped]
  public decimal Low => CalcPriceDecimal(LowStore);

  public int VolumeStore { get; private set; }
  [NotMapped]
  public decimal Volume => CalcVolumeDecimal(VolumeStore);

  #region TimeFrame relation
  /// <summary>
  /// Id of <see cref="TimeFrame"/> realated to entity
  /// </summary>
  /// <value></value>
  [Required]
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
  [Required]
  /// <summary>
  /// Id of <see cref="ent.Instrument"/> of <see cref="Candle"/>
  /// </summary>
  /// <value></value>  
  public int InstrumentId { get; private set; }

  private ent.Instrument _instrument;
  /// <summary>
  /// <see cref="ent.Instrument"/> of <see cref="Candle"/>
  /// </summary>
  /// <value></value>  
  public virtual ent.Instrument Instrument
  {
    get => _instrument;
    private set
    {
      InstrumentId = value.Id;
      _instrument = value;
    }
  }
  #endregion

  private decimal CalcPriceDecimal(int value_full)
  {
    if (Instrument == null)
    {
      throw new ArgumentException("To get price, you must load Instrument navigation property", nameof(Instrument));
    }
    return CalcDecimal(value_full, Instrument.PriceDecimalLen);
  }

  private decimal CalcVolumeDecimal(int value_full)
  {
    if (Instrument == null)
    {
      throw new ArgumentException("To get price, you must load Instrument navigation property", nameof(Instrument));
    }
    return CalcDecimal(value_full, Instrument.VolumeDecimalLen);
  }

  private decimal CalcDecimal(int value_full, int decimal_len)
  {
    return (decimal)value_full / ((decimal)Math.Pow(10, decimal_len));
  }
}