using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using Npgsql;
using Microsoft.Extensions.Hosting;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository;

namespace Instrument.Quote.Source.Configuration.DataBase.PostreSQL;

public static class Module
{

  public static DbConnectionStringBuilder GetConnectionStringBuilder(this IConfiguration configuration)
  {
    var _defConnection = configuration.GetConnectionString("DefaultConnection");
    var dbSuffix = configuration["ConnectionStrings:DbSuffix"];
    DbConnectionStringBuilder _connectionStringBuilder = new NpgsqlConnectionStringBuilder(_defConnection);
    if (dbSuffix != null)
      _connectionStringBuilder["Database"] += $"_{dbSuffix}";
    return _connectionStringBuilder;
  }
  public static IServiceCollection Register(this IServiceCollection sc)
  {
    using var sp = sc.BuildServiceProvider();
    var logger = sp.GetService<ILogger>();

    sc.AddSingleton<IConnectionStringSource, PGConnectionStringSource>();
    sc.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    sc.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));

    sc.AddScoped<ITransactionManager, TransactionManager<SrvDbContext>>();

    sc.AddDbContext<SrvDbContext>((provider, builder) =>
      {
        var connectionStringSource = provider.GetRequiredService<IConnectionStringSource>();
        var environment = provider.GetService<IHostEnvironment>();

        if (environment != null)
        {
          if (environment.IsDevelopment() || environment.IsEnvironment("Test"))
          {
            builder.EnableSensitiveDataLogging();
            builder.EnableDetailedErrors();
          }
        }

        logger?.LogInformation("PG host: {0}, db: {1}", connectionStringSource.Host, connectionStringSource.DataBase);
        builder.UseNpgsql(connectionStringSource.ConnectionString);
      });

    logger?.LogInformation("Migration - begin");
    sc.BuildServiceProvider().GetService<SrvDbContext>()!.Database.Migrate();
    logger?.LogInformation("Migration - done");
    return sc;
  }

  public static void DeleteDb(this IServiceProvider sp)
  {
    var logger = sp.GetService<ILogger>();
    logger?.LogWarning("Clearup DB");
    var environment = sp.GetService<IHostEnvironment>();
    if (environment == null)
      sp.GetRequiredService<ILogger>().LogWarning("Host environment is not defined, if it is prod FIX THIS");
    else if (environment.IsProduction())
      throw new ApplicationException("Delete Db is not allowed to use in production environment");
    using var dbContext = sp.GetRequiredService<SrvDbContext>();
    var result = dbContext.Database.EnsureDeleted();
    logger?.LogWarning("DB clearuped. Result: " + result);
  }
}
