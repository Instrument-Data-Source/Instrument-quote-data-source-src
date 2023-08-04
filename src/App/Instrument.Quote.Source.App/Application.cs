using Castle.Core.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App;


public static class Application
{
  public static IServiceCollection InitApp(this IServiceCollection sc)
  {
    Configuration.DataBase.PostreSQL.Module.Register(sc);
    Configuration.Jobs.QuartzModule.Module.AddQuartze(sc);
    Core.Module.Register(sc);
    return sc;
  }
}
