using System.Reflection;
using Instrument.Quote.Source.App.Core.CandleAggregate.Config;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Config;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model.Config;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Config;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using ent = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
namespace Instrument.Quote.Source.Configuration.DataBase;
public class SrvDbContext : DbContext
{
  public SrvDbContext(DbContextOptions options) : base(options)
  {

  }
  public DbSet<ent.Instrument> Instruments { get; set; }
  public DbSet<ent.InstrumentType> InstrumentTypes { get; set; }
  public DbSet<TimeFrame> TimeFrames { get; set; }
  public DbSet<Candle> Candles { get; set; }
  public DbSet<LoadedPeriod> LoadedPeriods { get; set; }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    => optionsBuilder
        .UseLazyLoadingProxies();

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    ManualyUseConfig(modelBuilder);
  }

  private void AutoUseConfig(ModelBuilder modelBuilder)
  {
    Console.WriteLine("Auto Config entity");
    IEnumerable<Assembly> usingAssemble = getUsingAssemblyList();

    NewMethod(modelBuilder, usingAssemble);
    Console.WriteLine("Auto Config entity - done");
  }

  private static void NewMethod(ModelBuilder modelBuilder, IEnumerable<Assembly> usingAssemble)
  {
    Console.WriteLine("Apply config in assembly");
    foreach (var assembly in usingAssemble)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
    Console.WriteLine("Apply config in assembly - done");
  }

  private void ManualyUseConfig(ModelBuilder modelBuilder)
  {
    Console.WriteLine("Manualy config entity");
    new InstrumentConfig().Configure(modelBuilder.Entity<ent.Instrument>());
    new InstrumentTypeConfig().Configure(modelBuilder.Entity<ent.InstrumentType>());
    new TimeFrameConfig().Configure(modelBuilder.Entity<TimeFrame>());
    new CandleConfig().Configure(modelBuilder.Entity<Candle>());
    new LoadedPeriodConfig().Configure(modelBuilder.Entity<LoadedPeriod>());
    Console.WriteLine("Manualy config entity - done");
  }
  private IEnumerable<Assembly> getUsingAssemblyList()
  {
    Console.WriteLine("Search assamblies");
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
    Console.WriteLine("Search assamblies - done");
    return usingAssemble;
  }
}
