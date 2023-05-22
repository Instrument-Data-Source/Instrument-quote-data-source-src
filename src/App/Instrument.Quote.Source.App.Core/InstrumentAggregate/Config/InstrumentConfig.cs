using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using model = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;

namespace Instrument.Quote.Source.App.Core.InstrumentAggregate.Config;

public class InstrumentConfig : IEntityTypeConfiguration<model.Instrument>
{
  public void Configure(EntityTypeBuilder<model.Instrument> modelBuilder)
  {

  }
}
