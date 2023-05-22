namespace Instrument.Quote.Source.App.Test.InstrumentAggregate.Service.IInstrumentSrvTest;

using System.Net;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Test.Tool;
using Instrument.Quote.Source.App.Test.Tool.SeedData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;
public class GetAll_Test : BaseTest
{
  ISeedContainer seeds;
  public GetAll_Test(ITestOutputHelper output) : base(nameof(GetAll_Test), output)
  {
    using var sc = serviceProvider.CreateScope();
    seeds = new SeedDataFactory(sc.ServiceProvider).InitInstrument().Seeds;
  }

  [Fact]
  public void WHEN_request_data_THEN_get_all()
  {
    // Array

    // Act
    using var act_sc = serviceProvider.CreateScope();
    var asserted_List = act_sc.ServiceProvider.GetService<IInstrumentSrv>().GetAllAsync().Result;

    // Assert 
    Assert.Equal(seeds.SeedInstrument.All.Count(), asserted_List.Count());
    foreach (var instrument in seeds.SeedInstrument.All)
    {
      Assert.Contains(instrument, asserted_List);
    }
  }
}