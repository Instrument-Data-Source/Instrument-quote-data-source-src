using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Config;

public class ChartConfig : IEntityTypeConfiguration<Chart>
{
  public void Configure(EntityTypeBuilder<Chart> modelBuilder)
  {
    modelBuilder
      .HasOne(e => e.Instrument)
      .WithMany(e => e.Charts)
      .HasForeignKey(e => e.InstrumentId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder
      .HasOne(e => e.TimeFrame)
      .WithMany(e => e.Charts)
      .HasForeignKey(e => e.TimeFrameId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Restrict);

    modelBuilder
      .HasIndex(e => new { e.InstrumentId, e.TimeFrameId })
      .IsUnique();
  }
}