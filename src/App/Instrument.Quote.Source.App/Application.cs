using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App;


public static class Application
{
  public static IServiceCollection InitApp(this IServiceCollection sc)
  {
    Instrument.Quote.Source.Configuration.DataBase.PostreSQL.Module.Register(sc);
    Instrument.Quote.Source.App.Core.Module.Register(sc);
    return sc;
  }
}
