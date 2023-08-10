using Microsoft.Extensions.Logging;
using Quartz.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.Tools;
public class ConsoleLogProvider : ILogProvider
{
  private readonly ITestOutputHelper output;

  public ConsoleLogProvider(ITestOutputHelper output)
  {
    this.output = output;
  }
  public Logger GetLogger(string name)
  {
    return (level, func, exception, parameters) =>
    {
      if (level >= Quartz.Logging.LogLevel.Info && func != null)
      {
        Console.WriteLine("[" + DateTime.Now.ToLongTimeString() + "] [" + level + "] " + func(), parameters);
      }
      return true;
    };
  }

  public IDisposable OpenNestedContext(string message)
  {
    throw new NotImplementedException();
  }

  public IDisposable OpenMappedContext(string key, object value, bool destructure = false)
  {
    throw new NotImplementedException();
  }
}