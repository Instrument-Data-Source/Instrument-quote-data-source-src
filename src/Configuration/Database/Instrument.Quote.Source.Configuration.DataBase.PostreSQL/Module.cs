using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data.Common;
using Npgsql;
using Microsoft.Extensions.Hosting;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;

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

        DbConnectionStringBuilder _connectionStringBuilder = new NpgsqlConnectionStringBuilder(_defConnection);

        if (!environment.IsProduction())
          _connectionStringBuilder["Database"] += environment.EnvironmentName;

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
}
