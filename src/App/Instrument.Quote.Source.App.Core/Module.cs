using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Service;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
namespace Instrument.Quote.Source.App.Core;

public static class Module
{
  public static IServiceCollection Register(this IServiceCollection sc)
  {
    sc.AddScoped<IInstrumentSrv, InstrumentSrv>();
    sc.AddScoped<ITimeFrameSrv, TimeFrameSrv>();
    sc.AddScoped<ICandleSrv, CandleSrv>();
    return sc;
  }
}