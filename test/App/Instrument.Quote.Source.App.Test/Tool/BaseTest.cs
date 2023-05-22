using Instrument.Quote.Source.Configuration.DataBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using app = Instrument.Quote.Source.App;

namespace Instrument.Quote.Source.App.Test.Tool;

public abstract class BaseTest : IDisposable
{
  protected readonly ITestOutputHelper output;
  protected IServiceProvider serviceProvider;
  protected ILogger test_logger;

  private static IServiceProvider BuildServiceProvider(string dbSuffix, ITestOutputHelper output)
  {
    var _configurationBuilder = new ConfigurationBuilder();
    _configurationBuilder.AddJsonFile("./appsettings.test.json");

    ServiceCollection sc = new ServiceCollection();
    sc.AddSingleton<IConfiguration>(_configurationBuilder.Build());
    sc.AddLogging(output);

    IHostEnvironment env = new HostingEnvironment { EnvironmentName = "test" + dbSuffix + DateTimeOffset.UtcNow + "_" + new Random().Next() };
    sc.AddSingleton<IHostEnvironment>(env);

    app.Application.InitApp(sc);

    return sc.BuildServiceProvider();
  }

  public BaseTest(string dbSuffix, ITestOutputHelper output)
  {
    this.output = output;
    serviceProvider = BuildServiceProvider(dbSuffix, output);
    test_logger = output.BuildLogger(dbSuffix);
  }

  protected SrvDbContext CreateContext()
  {
    return serviceProvider.GetService<SrvDbContext>()!;
  }
  public virtual void Dispose()
  {
    using (var _context = CreateContext())
    {
      _context.Database.EnsureDeleted();
    }
  }
}
