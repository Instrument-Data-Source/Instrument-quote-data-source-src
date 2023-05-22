using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Instrument.Quote.Source.App.Core.TimeFrameAggregate.Config;
public class TimeFrameConfig : IEntityTypeConfiguration<TimeFrame>
{
  public void Configure(EntityTypeBuilder<TimeFrame> entityBuilder)
  {
    foreach (var tf in TimeFrame.ToList())
    {
      entityBuilder.HasData(new TimeFrame(tf.EnumId));
    }
    entityBuilder.Property(e => e.Name).HasMaxLength(TimeFrame.NameLenght);
  }
}
