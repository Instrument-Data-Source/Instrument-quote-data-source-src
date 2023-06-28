using Microsoft.Extensions.DependencyInjection;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Service;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.Shared.FluentValidation.Extension;
using Microsoft.Extensions.Logging;

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
    sc.AddScoped<ITimeFrameSrv, TimeFrameSrv>();
    sc.AddScoped<ICandleSrv, CandleSrv>();
    logger?.LogInformation("Instrument.Quote.Source.App.Core.Module - Registered");
    return sc;
  }
}