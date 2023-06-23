using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.TestData;
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
  public Get_Test(ITestOutputHelper output)
  {
    srv = new CandleSrv(candleRep, loadedPeriodRep, instrumentRep, timeframeRep);
  }
/*
  [Fact]
  public void WHEN_request_existed_data_THEN_get_correct_data()
  {
    // Array
    var instument1 = new ent.Instrument("Inst1", "I1", 2, 3, new ent.InstrumentType(1));
    var period_arr = new[] { new LoadedPeriod(instument1, new TimeFrame(TimeFrame.Enum.D1), new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(2000, 1, 11).ToUniversalTime()) };
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());

    var candle_arr = CandleFactory.RandomCandles(10, new DateTime(2000, 1, 1).ToUniversalTime());
    candleRep.Table.Returns(candle_arr.BuildMock());

    var from_dt = new DateTime(2000, 1, 3).ToUniversalTime();
    var till_dt = new DateTime(2000, 1, 8).ToUniversalTime();

    // Act
    var asserted_res = srv.GetAsync(0, (int)TimeFrame.Enum.D1, from_dt, till_dt).Result;

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
    var instument1 = new ent.Instrument("Inst1", "I1", 2, 3, new ent.InstrumentType(1));
    var period_arr = new[] { new LoadedPeriod(instument1, new TimeFrame(TimeFrame.Enum.D1), new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(2000, 1, 11).ToUniversalTime()) };
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());

    var expected_arr = CandleFactory.RandomCandles(10, new DateTime(2000, 2, 1).ToUniversalTime());
    candleRep.Table.Returns(expected_arr.BuildMock());

    var from_dt = new DateTime(2000, 1, 1).ToUniversalTime();
    var till_dt = new DateTime(2000, 2, 8).ToUniversalTime();

    // Act

    // Assert
    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await srv.GetAsync(0, (int)TimeFrame.Enum.D1, from_dt, till_dt));
  }

  [Fact]
  public async Task WHEN_request_not_registered_instrument_THEN_return_failAsync()
  {
    // Array
    var instument1 = new ent.Instrument("Inst1", "I1", 2, 3, new ent.InstrumentType(1));
    var period_arr = new[] { new LoadedPeriod(instument1, new TimeFrame(TimeFrame.Enum.D1), new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(2000, 1, 11).ToUniversalTime()) };
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());

    var expected_arr = CandleFactory.RandomCandles(10, new DateTime(2000, 2, 1).ToUniversalTime());
    candleRep.Table.Returns(expected_arr.BuildMock());

    var from_dt = new DateTime(2000, 1, 1).ToUniversalTime();
    var till_dt = new DateTime(2000, 2, 8).ToUniversalTime();

    // Act

    // Assert
    await Assert.ThrowsAsync<ArgumentException>(async () => await srv.GetAsync(1, (int)TimeFrame.Enum.D1, from_dt, till_dt));
  }*/
}