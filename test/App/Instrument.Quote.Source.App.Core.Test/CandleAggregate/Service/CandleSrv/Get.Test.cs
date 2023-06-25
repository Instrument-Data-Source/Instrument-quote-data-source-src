using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Service.CandleSrvTest;

public class Get_Test
{
  ICandleSrv srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private TimeFrame mockTimeFrame = TimeFrame.Enum.D1.ToEntity();
  private MockPeriodFactory mockPeriodFactory;
  public Get_Test(ITestOutputHelper output)
  {
    srv = new CandleSrv(candleRep, loadedPeriodRep, instrumentRep, timeframeRep);
    mockPeriodFactory = new MockPeriodFactory(mockInstrument, mockTimeFrame);
  }

  [Fact]
  public void WHEN_request_existed_data_THEN_get_correct_data()
  {
    // Array
    var usedPeriod = mockPeriodFactory.CreatePeriod(new DateTime(2000, 1, 1), new DateTime(2000, 1, 15), initId: true);
    loadedPeriodRep.Table.Returns(new[] { usedPeriod }.BuildMock());

    candleRep.Table.Returns(usedPeriod.Candles.BuildMock());

    var from_dt = new DateTime(2000, 1, 3).ToUniversalTime();
    var till_dt = new DateTime(2000, 1, 8).ToUniversalTime();

    // Act
    var asserted_res = srv.GetAsync(mockInstrument.Id, mockTimeFrame.Id, from_dt, till_dt).Result;

    // Assert
    Assert.Equal(5, asserted_res.Count());
    Assert.NotNull(asserted_res.SingleOrDefault(e => e.DateTime == new DateTime(2000, 1, 3).ToUniversalTime()));
    Assert.NotNull(asserted_res.SingleOrDefault(e => e.DateTime == new DateTime(2000, 1, 4).ToUniversalTime()));
    Assert.NotNull(asserted_res.SingleOrDefault(e => e.DateTime == new DateTime(2000, 1, 5).ToUniversalTime()));
    Assert.NotNull(asserted_res.SingleOrDefault(e => e.DateTime == new DateTime(2000, 1, 6).ToUniversalTime()));
    Assert.NotNull(asserted_res.SingleOrDefault(e => e.DateTime == new DateTime(2000, 1, 7).ToUniversalTime()));
  }

  [Fact]
  public async Task WHEN_request_not_loaded_data_THEN_return_failAsync()
  {
    // Array
    var usedPeriod = mockPeriodFactory.CreatePeriod(new DateTime(2000, 2, 1), new DateTime(2000, 2, 15), initId: true);
    loadedPeriodRep.Table.Returns(new[] { usedPeriod }.BuildMock());

    candleRep.Table.Returns(usedPeriod.Candles.BuildMock());

    var from_dt = new DateTime(2000, 1, 1).ToUniversalTime();
    var till_dt = new DateTime(2000, 2, 8).ToUniversalTime();

    // Act

    // Assert
    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await srv.GetAsync(mockInstrument.Id, mockTimeFrame.Id, from_dt, till_dt));
  }

  [Fact]
  public async Task WHEN_request_not_registered_instrument_THEN_return_failAsync()
  {
    // Array
    var usedPeriod = mockPeriodFactory.CreatePeriod(new DateTime(2000, 1, 1), new DateTime(2000, 1, 15), initId: true);
    loadedPeriodRep.Table.Returns(new[] { usedPeriod }.BuildMock());

    candleRep.Table.Returns(usedPeriod.Candles.BuildMock());
    var from_dt = new DateTime(2000, 1, 1).ToUniversalTime();
    var till_dt = new DateTime(2000, 1, 8).ToUniversalTime();

    // Act

    // Assert
    await Assert.ThrowsAsync<ArgumentException>(async () => await srv.GetAsync(MockInstrument.Create().Id, mockTimeFrame.Id, from_dt, till_dt));
  }
}