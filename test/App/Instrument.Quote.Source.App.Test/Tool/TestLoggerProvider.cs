using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public static class ServiceCollectionExtension
{
  public static IServiceCollection AddLogging(this IServiceCollection sc, ITestOutputHelper output)
  {
    sc.AddLogging(b => b.AddProvider(new TestLoggerProvider(output)));
    return sc;
  }
}

public class TestLoggerProvider : ILoggerProvider
{
  private readonly ITestOutputHelper _output;

  public TestLoggerProvider(ITestOutputHelper output)
  {
    _output = output;
  }

  public ILogger CreateLogger(string categoryName)
  {
    return new TestLogger(_output);
  }

  public void Dispose()
  {
  }

  private class TestLogger : ILogger
  {
    private readonly ITestOutputHelper _output;

    public TestLogger(ITestOutputHelper output)
    {
      _output = output;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
      return null;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return true;
    }

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
      var message = formatter(state, exception);
      _output.WriteLine($"{logLevel}: {message}");
    }
  }
}
