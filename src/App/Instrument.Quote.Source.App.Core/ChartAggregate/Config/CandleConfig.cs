using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Config;

public class CandleConfig : IEntityTypeConfiguration<Candle>
{
  public void Configure(EntityTypeBuilder<Candle> modelBuilder)
  {
    modelBuilder
      .HasOne(e => e.Chart)
      .WithMany(e => e.Candles)
      .HasForeignKey(e => e.ChartId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder
      .HasIndex(e => new { e.ChartId, e.DateTime })
      .IsUnique();
  }
}