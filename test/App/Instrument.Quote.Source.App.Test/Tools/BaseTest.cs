using InsonusK.Xunit.ExpectationsTest;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.Tools;
[Collection("Host collection")]
public abstract class BaseTest : ExpectationsTestBase, IDisposable, IAsyncLifetime
{
  protected HostFixture hostFixture;
  protected BaseTest(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {
    output.BuildLogger().LogDebug("All tests from BaseTest inherit Collection Host collection to prevent parralelism. IHost could have only one instane at the time");
    hostFixture = new HostFixture(this.GetType().Name);
  }

  public async Task DisposeAsync()
  {
    await hostFixture.DisposeAsync();
  }

  public async Task InitializeAsync()
  {
    await hostFixture.InitializeAsync();
  }
  public new void Dispose()
  {
    hostFixture.Dispose();
  }
}