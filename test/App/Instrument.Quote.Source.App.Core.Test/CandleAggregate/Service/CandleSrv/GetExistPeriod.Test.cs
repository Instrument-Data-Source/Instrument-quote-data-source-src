using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Service.CandleSrvTest;

public class GetExistPeriod_Test : BaseTest<GetExistPeriod_Test>
{
  ICandleSrv srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private MockPeriodFactory periodFactory;

  public GetExistPeriod_Test(ITestOutputHelper output) : base(output)
  {
    srv = new CandleSrv(candleRep, loadedPeriodRep, instrumentRep, timeframeRep);
    periodFactory = new MockPeriodFactory(mockInstrument, TimeFrame.Enum.D1.ToEntity());
  }

  [Fact]
  public async void WHEN_period_exist_THEN_get_all_correct_periods()
  {
    // Array
    var d1_from = new DateTime(2000, 1, 1).ToUniversalTime();
    var d1_untill = new DateTime(2000, 1, 10).ToUniversalTime();
    var h1_from = new DateTime(2000, 2, 1).ToUniversalTime();
    var h1_untill = new DateTime(2000, 2, 10).ToUniversalTime();
    var instrument1 = MockInstrument.Create();
    var instrument2 = MockInstrument.Create();
    var period_arr = new[] {
        periodFactory.CreatePeriod(d1_from,d1_untill),
        periodFactory.SetTimeFrame(TimeFrame.Enum.H1.ToEntity()).CreatePeriod(h1_from, h1_untill),
        periodFactory.SetInstrument().CreatePeriod(new DateTime(2001, 1, 1).ToUniversalTime(), new DateTime(2001, 1, 11).ToUniversalTime())};
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());

    // Act
    var asserted_result = await srv.GetExistPeriodAsync(mockInstrument.Id);

    // Assert
    Expect("Result is success", () =>
      Assert.True(asserted_result.IsSuccess));

    Expect("Result contain 2 records", () =>
      Assert.Equal(2, asserted_result.Value.Count));


    ExpectGroup("Result contain D1 record", () =>
    {
      Expect("Response contain D1 key", () => Assert.Contains("D1", asserted_result.Value.Keys));
      var assertedPeriod = asserted_result.Value["D1"];
      Expect("From D1 is correct", () => Assert.Equal(d1_from, assertedPeriod.FromDate));
      Expect("Untill D1 is correct", () => Assert.Equal(d1_untill, assertedPeriod.UntillDate));
    });

    ExpectGroup("Result contain H1 record", () =>
    {
      Expect("Response contain H1 key", () => Assert.Contains("H1", asserted_result.Value.Keys));
      var assertedPeriod = asserted_result.Value["H1"];
      Expect("From W1 is correct", () => Assert.Equal(h1_from, assertedPeriod.FromDate));
      Expect("Untill W1 is correct", () => Assert.Equal(h1_untill, assertedPeriod.UntillDate));
    });
  }

  [Fact]
  public async void WHEN_periods_not_exist_but_insturment_existTHEN_return_empty()
  {
    // Array
    var period_arr = new[] {
          periodFactory.CreatePeriod(new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(2000, 1, 10).ToUniversalTime()),
          periodFactory.SetTimeFrame(TimeFrame.Enum.H1.ToEntity()).CreatePeriod(new DateTime(2000, 2, 1).ToUniversalTime(), new DateTime(2000, 2, 10).ToUniversalTime())};
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());

    var usedInstrument = MockInstrument.Create();

    instrumentRep.Table.Returns(new[] { mockInstrument, usedInstrument }.BuildMock());
    // Act
    var asserted_periods = await srv.GetExistPeriodAsync(usedInstrument.Id);

    // Assert
    Expect("Result is Success", () => Assert.True(asserted_periods.IsSuccess));
    Expect("Result value is empty", () => Assert.Empty(asserted_periods.Value));
  }

  [Fact]
  public async void WHEN_periods_not_exist_and_insturment_notexistTHEN_return_empty()
  {
    // Array
    var period_arr = new[] {
          periodFactory.CreatePeriod(new DateTime(2000, 1, 1).ToUniversalTime(), new DateTime(2000, 1, 10).ToUniversalTime()),
          periodFactory.SetTimeFrame(TimeFrame.Enum.H1.ToEntity()).CreatePeriod(new DateTime(2000, 2, 1).ToUniversalTime(), new DateTime(2000, 2, 3).ToUniversalTime())};
    loadedPeriodRep.Table.Returns(period_arr.BuildMock());

    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    var usedId = MockInstrument.Create().Id;

    // Act
    var asserted_periods = await srv.GetExistPeriodAsync(usedId);

    // Assert
    Expect("Result is not Succcess", () => Assert.False(asserted_periods.IsSuccess));
  }

}