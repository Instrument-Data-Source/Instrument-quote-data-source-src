namespace Instrument.Quote.Source.App.Core.ChartAggregate.Interface;

public interface IDecimalPartLongChecker
{
  bool IsPriceDecPartFit(decimal number);
  bool IsVolumeDecPartFit(decimal number);
}