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
  public static IServiceCollection Register(this IServiceCollection sc)
  {
    using var sp = sc.BuildServiceProvider();
    var logger = sp.GetService<ILogger>();
    sc.AddDbContext<SrvDbContext>((provider, builder) =>
      {
        var config = provider.GetService<IConfiguration>();
        var environment = provider.GetService<IHostEnvironment>();

        var _defConnection = config.GetConnectionString("DefaultConnection");
        var dbSuffix = config["ConnectionStrings:DbSuffix"];
        DbConnectionStringBuilder _connectionStringBuilder = new NpgsqlConnectionStringBuilder(_defConnection);

        if (environment != null)
        {
          if (environment.IsDevelopment() || environment.IsEnvironment("Test"))
          {
            builder.EnableSensitiveDataLogging();
            builder.EnableDetailedErrors();
          }
        }

        if (dbSuffix != null)
          _connectionStringBuilder["Database"] += $"_{dbSuffix}";

        logger?.LogInformation("PG db - " + _connectionStringBuilder["Database"]);
        builder.UseNpgsql(_connectionStringBuilder.ConnectionString);
      });

    logger?.LogInformation("Migration - begin");
    sc.BuildServiceProvider().GetService<SrvDbContext>()!.Database.Migrate();
    logger?.LogInformation("Migration - done");

    sc.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    sc.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
    sc.AddScoped<ITransactionManager, TransactionManager<SrvDbContext>>();
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
