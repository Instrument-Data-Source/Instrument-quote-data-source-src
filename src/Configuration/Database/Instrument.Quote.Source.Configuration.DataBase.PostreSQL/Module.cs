﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using Npgsql;
using Microsoft.Extensions.Hosting;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.Logging;

namespace Instrument.Quote.Source.Configuration.DataBase.PostreSQL;

public static class Module
{
  public static IServiceCollection Register(this IServiceCollection sc)
  {
    sc.AddDbContext<SrvDbContext>((provider, builder) =>
      {
        var config = provider.GetService<IConfiguration>();
        var environment = provider.GetService<IHostEnvironment>();

        var _defConnection = config.GetConnectionString("DefaultConnection");
        var dbSuffix = config["ConnectionStrings:DbSuffix"];
        DbConnectionStringBuilder _connectionStringBuilder = new NpgsqlConnectionStringBuilder(_defConnection);

        if (environment != null && !environment.IsProduction())
          _connectionStringBuilder["Database"] += $"_{environment.EnvironmentName}";

        if (dbSuffix != null)
          _connectionStringBuilder["Database"] += $"_{dbSuffix}";

        builder.UseNpgsql(_connectionStringBuilder.ConnectionString);

        //if (environment.IsDevelopment())
        //  builder.EnableSensitiveDataLogging();

        builder.EnableDetailedErrors();
      });

    sc.BuildServiceProvider().GetService<SrvDbContext>().Database.Migrate();

    sc.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
    sc.AddScoped(typeof(IReadRepository<>), typeof(EfRepository<>));
    return sc;
  }

  public static void InitDb(this IServiceProvider sp)
  {
    using var dbContext = sp.GetRequiredService<SrvDbContext>();
    dbContext.Database.Migrate();
  }

#if DEBUG
  public static void DeleteDb(this IServiceProvider sp)
  {
    var environment = sp.GetService<IHostEnvironment>();
    if (environment == null)
      sp.GetRequiredService<ILogger>().LogWarning("Host environment is not defined, if it is prod FIX THIS");
    else if (environment.IsProduction())
      throw new ApplicationException("Delete Db is not allowed to use in production environment");
    using var dbContext = sp.GetRequiredService<SrvDbContext>();
    dbContext.Database.EnsureDeleted();
  }
#endif
}
