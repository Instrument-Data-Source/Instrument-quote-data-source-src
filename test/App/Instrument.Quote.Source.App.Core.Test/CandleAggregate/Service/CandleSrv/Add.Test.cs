using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Service.CandleSrvTest;
public class Add_Test
{
  ICandleSrv srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  public Add_Test(ITestOutputHelper output)
  {
    srv = new CandleSrv(candleRep, loadedPeriodRep, instrumentRep);
  }

  [Fact]
  public void WHEN_add_for_empty_instrument_THEN_Ok()
  {
    // Array

    // Act
    //srv.Add(1, 1, new DateTime(2020, 1, 1).ToUniversalTime(), new DateTime(2020, 1, 1).ToUniversalTime())
    // Assert 
  }
}