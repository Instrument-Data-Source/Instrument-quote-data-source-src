using System.Reflection;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ent = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
namespace Instrument.Quote.Source.Configuration.DataBase;
public class SrvDbContext : DbContext
{
  private readonly ILogger<SrvDbContext>? logger = null;

  public SrvDbContext(DbContextOptions options, ILogger<SrvDbContext> logger) : this(options)
  {
    this.logger = logger;
  }
  public SrvDbContext(DbContextOptions options) : base(options)
  {

  }
  public DbSet<ent.Instrument> Instruments { get; set; }
  public DbSet<ent.InstrumentType> InstrumentTypes { get; set; }
  public DbSet<TimeFrame> TimeFrames { get; set; }
  public DbSet<Candle> Candles { get; set; }
  public DbSet<JoinedCandle> JoinedCandles { get; set; }
  public DbSet<Chart> Charts { get; set; }
  //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  //  => optionsBuilder
  //      .UseLazyLoadingProxies();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    AutoUseConfig(modelBuilder);
    //ManualyUseConfig(modelBuilder);
  }

  private void AutoUseConfig(ModelBuilder modelBuilder)
  {
    logger?.LogInformation("Auto Config entity");
    IEnumerable<Assembly> usingAssemble = getUsingAssemblyList();

    ApplyConfigFromAssemlies(modelBuilder, usingAssemble);
    logger?.LogInformation("Auto Config entity - done");
  }

  private void ApplyConfigFromAssemlies(ModelBuilder modelBuilder, IEnumerable<Assembly> usingAssemble)
  {
    logger?.LogInformation("Apply config in assembly");
    foreach (var assembly in usingAssemble)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
    logger?.LogInformation("Apply config in assembly - done");
  }

  private IEnumerable<Assembly> getUsingAssemblyList()
  {
    logger?.LogInformation("Search assamblies");
    HashSet<Assembly> usingAssemble = new HashSet<Assembly>();
    foreach (var propInfo in this.GetType().GetProperties())
    {
      if (propInfo.PropertyType.IsGenericType
            && propInfo.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
      {
        var assamble = propInfo.PropertyType.GetGenericArguments()[0].Assembly;

        if (!usingAssemble.Contains(assamble))
        {
          usingAssemble.Add(assamble);
        }
      }
    }
    logger?.LogInformation("Search assamblies - done");
    return usingAssemble;
  }
}
