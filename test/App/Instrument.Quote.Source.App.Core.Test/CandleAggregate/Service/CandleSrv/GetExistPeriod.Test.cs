using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Service.CandleSrvTest;

public class GetExistPeriod_Test
{
  ICandleSrv srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  public GetExistPeriod_Test(ITestOutputHelper output)
  {
    srv = new CandleSrv(candleRep, loadedPeriodRep, instrumentRep);
  }

  [Fact]
  public void WHEN_period_exist_THEN_get_all_correct_periods()
  {
    // Array
    var d1_from = new DateTime(2000, 1, 1).ToUniversalTime();
    var d1_untill = new DateTime(2000, 1, 10).ToUniversalTime();
    var w1_from = new DateTime(2000, 2, 1).ToUniversalTime();
    var w1_untill = new DateTime(2000, 2, 10).ToUniversalTime();
    var period_arr = new[] {
        new LoadedPeriod(0, (int)TimeFrame.Enum.D1, d1_from, d1_untill),
        new LoadedPeriod(0, (int)TimeFrame.Enum.W1, w1_from, w1_untill),
        new LoadedPeriod(1, (int)TimeFrame.Enum.D1, new DateTime(2001, 1, 1).ToUniversalTime(), new DateTime(2001, 1, 11).ToUniversalTime()) };
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());

    var expected_periods = new Dictionary<string, PeriodResponseDto>();
    expected_periods["D1"] = new PeriodResponseDto() { FromDate = d1_from, UntillDate = d1_untill };
    expected_periods["W1"] = new PeriodResponseDto() { FromDate = w1_from, UntillDate = w1_untill };

    // Act
    var asserted_result = srv.GetExistPeriodAsync(0).Result;

    // Assert
    Assert.True(asserted_result.IsSuccess);
    Assert.Equal(2, asserted_result.Value.Count);
    Assert.Equal(expected_periods["D1"], asserted_result.Value["D1"]);
    Assert.Equal(expected_periods["W1"], asserted_result.Value["W1"]);
  }

  [Fact]
  public void WHEN_periods_not_exist_but_insturment_existTHEN_return_empty()
  {
    // Array
    var period_arr = new[] {
        new LoadedPeriod(2, (int)TimeFrame.Enum.D1, new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(2000, 1, 10).ToUniversalTime()),
        new LoadedPeriod(2, (int)TimeFrame.Enum.W1, new DateTime(2000, 2, 1).ToUniversalTime(), new DateTime(2000, 2, 10).ToUniversalTime()),
        new LoadedPeriod(1, (int)TimeFrame.Enum.D1, new DateTime(2001, 1, 1).ToUniversalTime(), new DateTime(2001, 1, 11).ToUniversalTime()) };
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());
    var instrument_arr = new[]{
      new ent.Instrument("isntrument1","i1",2,2,1)
    };
    instrumentRep.Table.Returns(instrument_arr.BuildMock());
    // Act
    var asserted_periods = srv.GetExistPeriodAsync(0).Result;

    // Assert
    Assert.True(asserted_periods.IsSuccess);
    Assert.Equal(0, asserted_periods.Value.Count);
  }
  [Fact]
  public void WHEN_periods_not_exist_and_insturment_notexistTHEN_return_empty()
  {
    // Array
    var period_arr = new[] {
        new LoadedPeriod(2, (int)TimeFrame.Enum.D1, new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(2000, 1, 10).ToUniversalTime()),
        new LoadedPeriod(2, (int)TimeFrame.Enum.W1, new DateTime(2000, 2, 1).ToUniversalTime(), new DateTime(2000, 2, 10).ToUniversalTime()),
        new LoadedPeriod(1, (int)TimeFrame.Enum.D1, new DateTime(2001, 1, 1).ToUniversalTime(), new DateTime(2001, 1, 11).ToUniversalTime()) };
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());
    var instrument_arr = new[]{
      new ent.Instrument("isntrument1","i1",2,2,1)
    };
    instrumentRep.Table.Returns(instrument_arr.BuildMock());
    // Act
    var asserted_periods = srv.GetExistPeriodAsync(10).Result;

    // Assert
    Assert.False(asserted_periods.IsSuccess);
  }
}