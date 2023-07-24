using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Config;

public class JoinedChartConfig : IEntityTypeConfiguration<JoinedChart>
{
  public void Configure(EntityTypeBuilder<JoinedChart> modelBuilder)
  {
    modelBuilder
      .HasOne(e => e.BaseChart)
      .WithMany(e => e.JoinedCharts)
      .HasForeignKey(e => e.BaseChartId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Cascade);

    modelBuilder
      .HasOne(e => e.TargetTimeFrame)
      .WithMany(e => e.JoinedCharts)
      .HasForeignKey(e => e.TargetTimeFrameId)
      .IsRequired()
      .OnDelete(DeleteBehavior.Restrict);

    modelBuilder
      .HasIndex(e => new { e.BaseChartId, e.TargetTimeFrameId })
      .IsUnique();
  }
}