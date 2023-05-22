using System.ComponentModel.DataAnnotations;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase;
using Microsoft.EntityFrameworkCore;
namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;

/// <summary>
/// Instrument entity
/// </summary>
[Index(nameof(Code), IsUnique = true)]
public class Instrument : EntityBase
{
  public Instrument(
    string name,
    string code,
    byte priceDecimalLen,
    byte volumeDecimalLen,
    int instrumentTypeId)
  {
    if (string.IsNullOrEmpty(name))
    {
      throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
    }

    if (string.IsNullOrEmpty(code))
    {
      throw new ArgumentException($"'{nameof(code)}' cannot be null or empty.", nameof(code));
    }

    Name = name;
    Code = code;
    PriceDecimalLen = priceDecimalLen;
    VolumeDecimalLen = volumeDecimalLen;
    InstrumentTypeId = instrumentTypeId;
  }
  public Instrument(
      string name,
      string code,
      byte priceDecimalLen,
      byte volumeDecimalLen,
      InstrumentType instrumentType) :
    this(name, code, priceDecimalLen, volumeDecimalLen, instrumentType.Id)
  {
    InstrumentType = instrumentType;
  }
  /// <summary>
  /// Full name of instrument
  /// </summary>
  /// <value></value>
  [Required]
  [MaxLength(50)]
  public string Name { get; private set; }

  /// <summary>
  /// Short name of instrument
  /// </summary>
  /// <value></value>
  [Required]
  [MaxLength(10)]
  public string Code { get; private set; }

  /// <summary>
  /// Lenght of decimal part in Parice value
  /// </summary>
  /// <value></value>
  [Required]
  public byte PriceDecimalLen { get; private set; }

  /// <summary>
  /// Lenght of decimal part in Volume value
  /// </summary>
  /// <value></value>
  [Required]
  public byte VolumeDecimalLen { get; private set; }

  private int _instrumentTypeId;
  /// <summary>
  /// Type Id of instrument
  /// </summary>
  /// <value></value>
  [Required]
  public int InstrumentTypeId
  {
    get => _instrumentTypeId;
    private set
    {
      _instrumentTypeId = value;
      if (value != _instrumentType?.Id)
      {
        _instrumentType = null;
      }
    }
  }

  private InstrumentType? _instrumentType;
  /// <summary>
  /// Type of instrument
  /// </summary>
  /// <value></value>
  public virtual InstrumentType? InstrumentType
  {
    get => _instrumentType;
    private set
    {
      _instrumentType = value;
      if (value != null)
      {
        InstrumentTypeId = value.Id;
      }
    }
  }
  private readonly List<LoadedPeriod> _loadedPeriods = new();
  public virtual IEnumerable<LoadedPeriod> LoadedPeriods => _loadedPeriods.AsReadOnly();
  private readonly List<Candle> _candles = new();
  public virtual IEnumerable<Candle> Candles => _candles.AsReadOnly();
}