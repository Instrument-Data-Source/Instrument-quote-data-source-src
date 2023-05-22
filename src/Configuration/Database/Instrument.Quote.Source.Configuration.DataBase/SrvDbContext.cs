using System.Reflection;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
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

    Console.WriteLine("Start search for assably");
    IEnumerable<Assembly> usingAssemble = getUsingAssemblyList();

    foreach (var assembly in usingAssemble)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(assembly);
    }
  }

  private IEnumerable<Assembly> getUsingAssemblyList()
  {
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

    return usingAssemble;
  }
}
