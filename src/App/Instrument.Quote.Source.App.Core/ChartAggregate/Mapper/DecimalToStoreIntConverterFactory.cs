using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;

public class DecimalToStoreIntConverterFactory : IDecimalPartLongCheckerFactory
{
  public IDecimalPartLongChecker GetChecker(ent.Instrument instrument)
  {
    return new DecimalToStoreIntConverter(instrument);
  }
}
