using System.ComponentModel;
using Alba;
using InsonusK.Xunit.ExpectationsTest;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
using Instrument.Quote.Source.Configuration.DataBase.PostreSQL;
using Microsoft.Extensions.Logging.Abstractions;

namespace Instrument.Quote.Source.App.Test.Tools;

public class HostFixture : IDisposable, IAsyncLifetime
{
  public HostFixture(string testName)
  {
    this.testName = testName;
  }
  private static int magicNumber = 0;
  protected string dbSufix { get; private set; }
  private readonly string testName;

  public IAlbaHost Host { get; private set; }
  public IServiceProvider Services => Host.Services;
  public async Task InitializeAsync()
  {
    dbSufix = GetDbSufix(testName);
    // Program.CreateHostBuilder() is the code from the WebApplication
    // that configures the HostBuilder for the system
    Host = await new HostBuilder()
          .ConfigureHostConfiguration(config => setupConfigBuider(dbSufix, config))
          .ConfigureServices((hostContext, services) =>
          {
            // Add required services to the ServiceCollection
            var hostEnvironment = Substitute.For<IHostEnvironment>();

            hostEnvironment.EnvironmentName.Returns("Test");

            services.AddSingleton<IHostEnvironment>(hostEnvironment);
            //Don't use ITestOutputHelper because it broke host building when run all test at once 
            //Quartz sheduler problem https://github.com/quartznet/quartznet/issues/1781
            services.AddLogging(builder =>
            {
              builder.SetMinimumLevel(hostContext.Configuration.GetSection("Logging:LogLevel:Default").Get<LogLevel>());
              builder.AddConsole();
            });
            App.Application.InitApp(services);
          })
          .StartAlbaAsync();
  }

  public async Task DisposeAsync()
  {
    Host?.Services.DeleteDb();
    await Host!.StopAsync();

  }
  public void Dispose()
  {
    Host?.Dispose();
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

  private string GetDbSufix(string testName)
  {
    Interlocked.Increment(ref magicNumber);
    var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
    var sufix = $"{testName}_{magicNumber.ToString()}_{timeStamp}";
    return sufix;
  }


}
