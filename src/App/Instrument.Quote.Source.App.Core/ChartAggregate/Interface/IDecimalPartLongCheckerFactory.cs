namespace Instrument.Quote.Source.App.Core.ChartAggregate.Interface;

public interface IDecimalPartLongCheckerFactory
{
  public IDecimalPartLongChecker GetChecker(ent.Instrument instrument);
}