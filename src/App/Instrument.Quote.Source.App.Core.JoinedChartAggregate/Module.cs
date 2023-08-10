using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Config;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate;
public static class Module
{
  public static IServiceCollection Register(this IServiceCollection sc)
  {
    using var sp = sc.BuildServiceProvider();
    var logger = sp.GetService<ILogger>();
    logger?.LogInformation("Instrument.Quote.Source.App.Core.JoinedChartAggregate - Registering");

    var config = sp.GetRequiredService<IConfiguration>();

    sc.AddOptions<JoinedChartModuleConfig>()
      .Bind(config.GetSection(nameof(JoinedChartModuleConfig)))
      .ValidateDataAnnotations()
      .ValidateOnStart();

    sc.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Module).Assembly));

    sc.AddScoped<IBasePeriodSplitter, BasePeriodSplitter>();
    sc.AddScoped<IChartJoiner, ChartJoiner>();
    sc.AddScoped<IJoinedChartFactory, JoinedChart.Factory>();
    sc.AddScoped<IJoinedChartManager, JoinedChart.Manager>();

    logger?.LogInformation("Instrument.Quote.Source.App.Core.JoinedChartAggregate - Registered");
    return sc;
  }
}