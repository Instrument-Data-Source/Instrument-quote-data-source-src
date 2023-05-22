using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.Configuration.DataBase.PostreSQL;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
