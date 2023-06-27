using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.Configuration.DataBase.PostreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.Tools;

public class BaseTest<T> where T : BaseTest<T>
{
  protected readonly ILogger<T> logger;

  public BaseTest(ITestOutputHelper output)
  {
    this.logger = output.BuildLoggerFor<T>();
  }

  protected void Expect(string exception, Action assertAction)
  {
    try
    {
      assertAction.Invoke();
    }
    catch (System.Exception ex)
    {
      this.logger.LogInformation($"{exception} - Failed");
      throw ex;
    }
    this.logger.LogInformation($"{exception} - Checked");
  }
  protected void Expect<TRet>(string exception, Func<TRet> assertFunc, out TRet returnObject)
  {
    try
    {
      returnObject = assertFunc.Invoke();
    }
    catch (System.Exception ex)
    {
      this.logger.LogInformation($"{exception} - Failed");
      throw ex;
    }
    this.logger.LogInformation($"{exception} - Checked");
  }
  protected void ExpectGroup(string exception, Action assertAction)
  {
    this.logger.LogInformation($"{exception} - Checking...");
    try
    {
      assertAction.Invoke();
    }
    catch (System.Exception ex)
    {
      this.logger.LogInformation($"{exception} - Failed");
      throw ex;
    }
    this.logger.LogInformation($"{exception} - Checked");
  }
  protected void ExpectGroup<TRet>(string exception, Func<TRet> assertFunc, out TRet returnObject)
  {
    this.logger.LogInformation($"{exception} - Checking");
    try
    {
      returnObject = assertFunc.Invoke();
    }
    catch (System.Exception ex)
    {
      this.logger.LogInformation($"{exception} - Failed");
      throw ex;
    }
    this.logger.LogInformation($"{exception} - Checked");
  }
}


public abstract class BaseDbTest<T> : BaseTest<T>, IDisposable where T : BaseTest<T>
{
  protected ServiceProvider global_sp;

  private static IConfiguration GetConfig(string dbSuffix)
  {
    var _configurationBuilder = new ConfigurationBuilder();
    _configurationBuilder.AddJsonFile("./appsettings.test.json");
    _configurationBuilder.AddEnvironmentVariables();
    var dict = new Dictionary<string, string>
      {
          {"ConnectionStrings:DbSuffix", dbSuffix}
      };
    _configurationBuilder.AddInMemoryCollection(dict);
    Console.WriteLine(_configurationBuilder.Build().GetConnectionString("DefaultConnection"));
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
    using var scope = global_sp.CreateScope();
    var _sp = scope.ServiceProvider;
#if DEBUG
    _sp.DeleteDb();
#endif
  }

}
