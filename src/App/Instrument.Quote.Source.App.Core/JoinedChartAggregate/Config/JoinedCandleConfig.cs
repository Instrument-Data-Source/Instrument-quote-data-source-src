using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Config;

public class JoinedCandleConfig : IEntityTypeConfiguration<JoinedCandle>
{
  public void Configure(EntityTypeBuilder<JoinedCandle> modelBuilder)
  {
    //modelBuilder
    //  .HasOne(e => e.JoinedChart)
    //  .WithMany(e => e.JoinedCandles)
    //  .HasForeignKey(e => e.JoinedChartId)
    //  .IsRequired()
    //  .OnDelete(DeleteBehavior.Cascade);

    modelBuilder
      .HasIndex(e => new { e.JoinedChartId, e.StepDateTime })
      .IsUnique();
  }
}