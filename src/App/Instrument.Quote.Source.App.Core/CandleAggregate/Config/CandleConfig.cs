using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrument.Quote.Source.App.Core.CandleAggregate.Config;

public class CandleConfig : IEntityTypeConfiguration<Candle>
{
  public void Configure(EntityTypeBuilder<Candle> modelBuilder)
  {
    modelBuilder
      .HasOne(e => e.Instrument)
      .WithMany(e => e.Candles)
      .HasForeignKey(e => e.InstrumentId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder
      .HasOne(e => e.TimeFrame)
      .WithMany(e => e.Candles)
      .HasForeignKey(e => e.TimeFrameId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Restrict);

    modelBuilder
      .HasIndex(e => new { e.InstrumentId, e.TimeFrameId, e.DateTime })
      .IsUnique();
  }
}