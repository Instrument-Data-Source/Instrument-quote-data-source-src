using Microsoft.Extensions.Configuration;

namespace Instrument.Quote.Source.Configuration.DataBase;
public interface IConnectionStringSource
{
  public string ConnectionString { get; }
  public string Host { get; }
  public string DataBase { get; }
}