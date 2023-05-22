using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Config;

public class LoadedPeriodConfig : IEntityTypeConfiguration<LoadedPeriod>
{
  public void Configure(EntityTypeBuilder<LoadedPeriod> modelBuilder)
  {
    modelBuilder
      .HasOne(e => e.Instrument)
      .WithMany(e => e.LoadedPeriods)
      .HasForeignKey(e => e.InstrumentId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder
      .HasOne(e => e.TimeFrame)
      .WithMany(e => e.LoadedPeriods)
      .HasForeignKey(e => e.TimeFrameId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Restrict);

    modelBuilder
      .HasIndex(e => new { e.InstrumentId, e.TimeFrameId })
      .IsUnique();
  }
}