using Quartz;
using Microsoft.Extensions.DependencyInjection;
using AppAny.Quartz.EntityFrameworkCore.Migrations;
using Instrument.Quote.Source.Configuration.DataBase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Instrument.Quote.Source.Configuration.Jobs.QuartzModule;

public static class Module
{
  public static IServiceCollection AddQuartz(this IServiceCollection services)
  {
    using ServiceProvider sp = services.BuildServiceProvider();
    var connectionStringSource = sp.GetRequiredService<IConnectionStringSource>();
    Quartz.Logging.LogContext.SetCurrentLogProvider(NullLoggerFactory.Instance);
    services.AddQuartz(q =>
      {
        q.UseMicrosoftDependencyInjectionJobFactory();
        q.UsePersistentStore(options =>
        {
          options.Properties.Add("quartz.jobStore.tablePrefix", "quartz.qrtz_");
          options.PerformSchemaValidation = true;
          //options.UseProperties = true;
          options.UseJsonSerializer();
          options.UsePostgres(connectionStringSource.ConnectionString);
        });
      });

    services.AddQuartzHostedService(opt =>
    {
      opt.WaitForJobsToComplete = true;
      opt.AwaitApplicationStarted = true;
    });

    return services;
  }
}