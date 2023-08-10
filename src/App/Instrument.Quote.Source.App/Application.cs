using Castle.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App;


public static class Application
{
  public static IServiceCollection InitApp(this IServiceCollection sc)
  {
    Configuration.DataBase.PostreSQL.Module.Register(sc);
    Configuration.Jobs.QuartzModule.Module.AddQuartz(sc);
    Core.Module.Register(sc);
    Core.JoinedChartAggregate.Module.Register(sc);
    return sc;
  }
}
