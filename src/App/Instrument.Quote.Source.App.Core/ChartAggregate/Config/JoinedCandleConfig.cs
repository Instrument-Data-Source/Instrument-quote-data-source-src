using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrument.Quote.Source.App.Core.ChartAggregate.Config;

public class JoinedCandleConfig : IEntityTypeConfiguration<JoinedCandle>
{
  public void Configure(EntityTypeBuilder<JoinedCandle> modelBuilder)
  {
    modelBuilder
      .HasOne(e => e.Chart)
      .WithMany(e => e.JoinedCandles)
      .HasForeignKey(e => e.ChartId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder
      .HasOne(e => e.TargetTimeFrame)
      .WithMany(e=>e.JoinedCandles)
      .HasForeignKey(e => e.TargetTimeFrameId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Restrict);

    modelBuilder
      .HasIndex(e => new { e.ChartId, e.TargetTimeFrameId, e.StepDateTime })
      .IsUnique();
  }
}