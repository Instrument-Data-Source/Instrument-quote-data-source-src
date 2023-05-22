using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using model = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Model.Config;

public class InstrumentTypeConfig : IEntityTypeConfiguration<model.InstrumentType>
{
  public void Configure(EntityTypeBuilder<model.InstrumentType> modelBuilder)
  {
    foreach (var item in model.InstrumentType.ToList())
    {
      modelBuilder.HasData(new model.InstrumentType(item.Id));
    }

    modelBuilder.Property(e => e.Name).HasMaxLength(InstrumentType.NameLenght);
    modelBuilder.HasMany(e => e.Instruments)
                .WithOne(e => e.InstrumentType)
                .HasForeignKey(e => e.InstrumentTypeId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);
  }
}
