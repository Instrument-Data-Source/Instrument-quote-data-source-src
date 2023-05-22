using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Instrument.Quote.Source.Configuration.DataBase;

namespace Instrument.Quote.Source.Configuration.DataBase.Migrations.Factory;

public class ContextFactory : IDesignTimeDbContextFactory<SrvDbContext>
{
  private IConfigurationBuilder _configurationBuilder;

  public ContextFactory()
  {
    _configurationBuilder = new ConfigurationBuilder();
    _configurationBuilder.AddJsonFile("./appsettings.Development.json");
  }

  public SrvDbContext CreateDbContext(string[] args)
  {
    return CreateDbContext();
  }

  public SrvDbContext CreateDbContext()
  {
    var _config = _configurationBuilder.Build();
    string connectionString = _config.GetConnectionString("DefaultConnection");

    var optionsBuilder = new DbContextOptionsBuilder<SrvDbContext>();
    optionsBuilder.UseNpgsql(connectionString);

    return new SrvDbContext(optionsBuilder.Options);
  }
}