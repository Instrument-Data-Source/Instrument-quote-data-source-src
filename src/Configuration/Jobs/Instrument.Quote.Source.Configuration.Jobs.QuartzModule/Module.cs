using Quartz;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;

namespace Instrument.Quote.Source.Configuration.Jobs.QuartzModule;

public static class Module
{
  public static void AddSrvQuartz(this ModelBuilder modelBuilder)
  {
    modelBuilder.AddQuartz(builder => builder.UsePostgreSql());
  }
  public static IServiceCollection AddQuartze(this IServiceCollection services)
  {
    var configuration = services.BuildServiceProvider().GetRequiredService<IConfiguration>();
    services.AddQuartz(q =>
      {
        q.UseMicrosoftDependencyInjectionJobFactory();
        q.UsePersistentStore(options =>
        {
          options.Properties.Add("quartz.jobStore.tablePrefix", "quartz.qrtz_");
          options.PerformSchemaValidation = true;
          options.UseProperties = true;
          options.UseJsonSerializer();
          options.UsePostgres(configuration.GetConnectionString("QuartzConnection"));
        });
      });
    IServiceCollection serviceCollection = services.AddQuartzHostedService(opt =>
    {
      opt.WaitForJobsToComplete = true;
    });

    return services;
  }

}