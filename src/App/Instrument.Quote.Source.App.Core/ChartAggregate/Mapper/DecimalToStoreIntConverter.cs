using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
public class DecimalToStoreIntConverter : IDecimalPartLongChecker
{
  private readonly ent.Instrument instrument;

  public DecimalToStoreIntConverter(ent.Instrument instrument)
  {
    this.instrument = instrument;
  }
  public int PriceToInt(decimal value)
  {
    return CalcInt(value, instrument.PriceDecimalLen);
  }
  public bool TryPriceToInt(decimal value, out int price)
  {
    return TryCalcInt(value, instrument.PriceDecimalLen, out price);
  }
  public decimal PriceToDecimal(int value)
  {
    return CalcDecimal(value, instrument.PriceDecimalLen);
  }
  public int VolumeToInt(decimal value)
  {
    return CalcInt(value, instrument.VolumeDecimalLen);
  }

  public bool TryVolumeToInt(decimal value, out int volume)
  {
    return TryCalcInt(value, instrument.VolumeDecimalLen, out volume);
  }

  public decimal VolumeToDecimal(int value_full)
  {
    return CalcDecimal(value_full, instrument.VolumeDecimalLen);
  }

  private decimal CalcDecimal(int value_full, int decimal_len)
  {
    return (decimal)value_full / ((decimal)Math.Pow(10, decimal_len));
  }
  private bool TryCalcInt(decimal value, int decimal_len, out int result)
  {
    result = 0;
    if (IsDecimalPartLonger(value, decimal_len))
      return false;
    result = (int)(value * (int)Math.Pow(10, decimal_len));
    return true;
  }

  private int CalcInt(decimal value, int decimal_len)
  {
    if (TryCalcInt(value, decimal_len, out var result))
      return result;
    throw new ArgumentOutOfRangeException(nameof(value), value, "Decimal value has more digits that allowed by Instrument");
  }

  private bool IsDecimalPartLonger(decimal number, int digitCount)
  {
    string[] parts = number.ToString().Split('.');
    if (parts.Length == 1)
      return false;

    string decimalPart = parts[1];
    return decimalPart.Length > digitCount;
  }

  public bool IsPriceDecPartFit(decimal price)
  {
    return !IsDecimalPartLonger(price, instrument.PriceDecimalLen);
  }

  public bool IsVolumeDecPartFit(decimal volume)
  {
    return !IsDecimalPartLonger(volume, instrument.VolumeDecimalLen);
  }
}
