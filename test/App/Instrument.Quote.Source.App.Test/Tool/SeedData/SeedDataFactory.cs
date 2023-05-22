using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Test.Tool.SeedData.Dto;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App.Test.Tool.SeedData;

public class SeedDataFactory
{
  private readonly IServiceProvider serviceProvider;
  private readonly SeedContainer seedContainer = new SeedContainer();
  public ISeedContainer Seeds => seedContainer;

  public SeedDataFactory(IServiceProvider serviceProvider)
  {
    this.serviceProvider = serviceProvider;
  }

  public SeedDataFactory InitInstrument()
  {
    var srv = this.serviceProvider.GetService<IInstrumentSrv>();
    if (seedContainer.SeedInstrument != null) return this;

    var seedInst = new SeedInstrument();

    var inst1 = DtoFactory.Instrument1.NewInstrumentDto;
    seedInst.Instrument1 = srv.CreateInstrumentAsync(inst1).Result;

    var inst2 = DtoFactory.Instrument2.NewInstrumentDto;
    seedInst.Instrument2 = srv.CreateInstrumentAsync(inst2).Result;

    seedContainer.SeedInstrument = seedInst;
    return this;
  }
}
