using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator.Attribute;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Model;

/// <summary>
/// Quotes of <see cref="ent.Instrument"/> 
/// <see cref="ADR-001 Decimal in candle value">ADR-001 Decimal in candle value</see>
/// </summary>
public partial class Candle : EntityBaseExt<Candle>, Candle.IPayload
{

  [Required]
  [UTCKind]
  public DateTime DateTime { get; private set; }

  private int _OpenStore;
  [Required]
  [PriceGeLow]
  [PriceLeHigh]
  public int OpenStore { get; private set; }

  [Required]
  [PriceGeLow]
  [PriceLeHigh]
  public int CloseStore { get; private set; }

  [Required]
  [HighIsMax]
  public int HighStore { get; private set; }
  [Required]
  [LowIsMin]
  public int LowStore { get; private set; }

  [Range(0, int.MaxValue)]
  public int VolumeStore { get; private set; }

  private int _TimeFrameId;
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
      _TimeFrameId = value.Id;
      _TimeFrame = value;
    }
  }

  private int _InstrumentId;
  public int InstrumentId
  {
    get => _InstrumentId;
    private set
    {
      _Instrument = null;
      _InstrumentId = value;
    }
  }

  private ent.Instrument _Instrument;
  /// <summary>
  /// <see cref="ent.Instrument"/> of <see cref="Candle"/>
  /// </summary>
  /// /// <value></value>  
  public virtual ent.Instrument Instrument
  {
    get => _Instrument; private set
    {
      _InstrumentId = value.Id;
      _Instrument = value;
    }
  }

  [NotMapped]
  public decimal Open => CalcPriceDecimal(OpenStore);
  [NotMapped]
  public decimal Close => CalcPriceDecimal(CloseStore);
  [NotMapped]
  public decimal High => CalcPriceDecimal(HighStore);
  [NotMapped]
  public decimal Low => CalcPriceDecimal(LowStore);
  [NotMapped]
  public decimal Volume => CalcVolumeDecimal(VolumeStore);

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