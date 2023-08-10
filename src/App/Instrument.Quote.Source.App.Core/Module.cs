using Microsoft.Extensions.DependencyInjection;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Service;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Core.ChartAggregate.Service;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;

namespace Instrument.Quote.Source.App.Core;

public static class Module
{
  public static IServiceCollection Register(this IServiceCollection sc)
  {
    using var sp = sc.BuildServiceProvider();
    var logger = sp.GetService<ILogger>();
    logger?.LogInformation("Instrument.Quote.Source.App.Core.Module - Registering");
    sc.AddScoped<IInstrumentSrv, InstrumentSrv>();
    sc.AddScoped<IReadInstrumentSrv, InstrumentSrv>();
    sc.AddScoped<IInstrumentTypeSrv, InstrumentTypeSrv>();
    sc.AddScoped<ITimeFrameSrv, TimeFrameSrv>();
    sc.AddScoped<IChartSrv, ChartSrv>();
    sc.AddScoped<ICandleSrv, CandlesSrv>();
    sc.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Module).Assembly));
    logger?.LogInformation("Instrument.Quote.Source.App.Core.Module - Registered");
    return sc;
  }
}