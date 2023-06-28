using Instrument.Quote.Source.Configuration.DataBase.PostreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.Tools;
public static class Counter
{
  private static int value = 0;

  public static int Value => value;

  public static int Next()
  {
    return Interlocked.Increment(ref value);
  }
}
public abstract class BaseDbTest<T> : BaseTest<T>, IDisposable where T : BaseTest<T>
{
  protected ServiceProvider global_sp;
  private int number = 1;
  private static IConfiguration GetConfig(string dbSuffix)
  {
    var _configurationBuilder = new ConfigurationBuilder();
    _configurationBuilder.AddJsonFile("./appsettings.test.json");
    _configurationBuilder.AddEnvironmentVariables();
    var cur_numb = Counter.Next();
    var dict = new Dictionary<string, string>
      {
          {"ConnectionStrings:DbSuffix", dbSuffix+cur_numb}
      };
    _configurationBuilder.AddInMemoryCollection(dict);
    Console.WriteLine(_configurationBuilder.Build().GetConnectionString("DefaultConnection"));
    Console.WriteLine("Db Suffix: " + dbSuffix);
    return _configurationBuilder.Build();
  }

  public BaseDbTest(ITestOutputHelper output) : base(output)
  {
    var sc = new ServiceCollection();

    sc.AddSingleton<IConfiguration>(GetConfig(typeof(T).Name));
    sc.AddLogging(builder =>
        {
          builder.AddXunit(output); // Add the xUnit logger
        });
    sc.AddSingleton<ILogger>(sp => output.BuildLogger());

    App.Application.InitApp(sc);
    global_sp = sc.BuildServiceProvider();
  }


  public virtual void Dispose()
  {
    DeleteDb();
  }

  private void DeleteDb()
  {
    using var scope = global_sp.CreateScope();
    var _sp = scope.ServiceProvider;
    _sp.DeleteDb();
  }
}
