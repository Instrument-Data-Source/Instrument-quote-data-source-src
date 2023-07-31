using InsonusK.Xunit.ExpectationsTest;
using Instrument.Quote.Source.Configuration.DataBase.PostreSQL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.Tools;
public abstract class BaseDbTest : ExpectationsTestBase, IDisposable
{
  private static int magicNumber = 0;
  public readonly IServiceProvider global_sp;
  private int number = 1;
  protected readonly string dbSufix;
  private static IConfiguration GetConfig(string dbSuffix)
  {
    var _configurationBuilder = new ConfigurationBuilder();
    setupConfigBuider(dbSuffix, _configurationBuilder);
    return _configurationBuilder.Build();
  }

  private static void setupConfigBuider(string dbSuffix, IConfigurationBuilder _configurationBuilder)
  {

    _configurationBuilder.AddJsonFile("./appsettings.test.json");
    _configurationBuilder.AddEnvironmentVariables();
    var dict = new Dictionary<string, string>
      {
          {"ConnectionStrings:DbSuffix", dbSuffix},
          {"ASPNETCORE_ENVIRONMENT", "Development"}
      };
    _configurationBuilder.AddInMemoryCollection(dict);

    //Console.WriteLine(_configurationBuilder.Build().GetConnectionString("DefaultConnection"));
    //Console.WriteLine("Db Suffix: " + dbSuffix);
  }

  private string GetDbSufix()
  {
    Interlocked.Increment(ref BaseDbTest.magicNumber);
    var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
    var sufix = $"{this.GetType().Name}_{magicNumber.ToString()}_{timeStamp}";
    return sufix;
  }
  public BaseDbTest(ITestOutputHelper output) : base(output)
  {
    dbSufix = GetDbSufix();
    var host = new HostBuilder()
          .ConfigureHostConfiguration(config => setupConfigBuider(dbSufix, config))
          .ConfigureServices((hostContext, services) =>
          {
            // Add required services to the ServiceCollection
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.EnvironmentName.Returns("Test");

            services.AddSingleton<IHostEnvironment>(hostEnvironment);
            services.AddSingleton<IConfiguration>(GetConfig(dbSufix));
            services.AddLogging(builder =>
                  {
                    builder.AddXunit(output); // Add the xUnit logger
                  });
            services.AddSingleton<ILogger>(sp => output.BuildLogger());
            App.Application.InitApp(services);
          })
          .Build();

    global_sp = host.Services;
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
