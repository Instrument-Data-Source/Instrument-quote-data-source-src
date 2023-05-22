using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.TestData;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
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
  public async Task WHEN_add_for_empty_instrument_THEN_OkAsync()
  {
    // Array
    var instument1 = new ent.Instrument("Inst1", "I1", 2, 3, new ent.InstrumentType(1));
    instrumentRep.Table.Returns(new[] { instument1 }.BuildMock());
    var expectedCandleDto = CandleFactory.RandomCandles(3, new DateTime(2020, 1, 1).ToUniversalTime()).Select(e => e.ToDto());
    var expectedFrom = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntill = new DateTime(2020, 1, 10).ToUniversalTime();

    loadedPeriodRep.Table.Returns(new List<LoadedPeriod>().BuildMock());

    // Act
    await srv.AddAsync(0, (int)TimeFrame.Enum.D1, expectedFrom, expectedUntill, expectedCandleDto);

    // Assert
    //loadedPeriodRep.Received().AddAsync(Arg.Any<LoadedPeriod>(), Arg.Any<IDbContextTransaction>(), Arg.Any<CancellationToken>());
    loadedPeriodRep.Received().AddAsync(Arg.Is<LoadedPeriod>(e =>
        e.FromDate == expectedFrom &&
        e.UntillDate == expectedUntill &&
        e.InstrumentId == 99 &&
        e.TimeFrameId == (int)TimeFrame.Enum.D1 &&
        e.Candles.Count() == expectedCandleDto.Count()),
    Arg.Any<IDbContextTransaction>(),
    Arg.Any<CancellationToken>());
  }
}