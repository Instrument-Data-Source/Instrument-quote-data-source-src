namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Service.CandleSrvTest;

using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;

public class GetAll_Test
{
  ICandleSrv srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private TimeFrame mockTimeFrame = TimeFrame.Enum.D1.ToEntity();
  private MockCandleFactory mockCandleFactory;
  public GetAll_Test(ITestOutputHelper output)
  {
    srv = new CandleSrv(candleRep, loadedPeriodRep, instrumentRep, timeframeRep);
    mockCandleFactory = new MockCandleFactory(mockInstrument, mockTimeFrame);
  }

  [Fact]
  public void WHEN_call_THEN_get_data_from_repository()
  {
    // Array
    var expected_arr = mockCandleFactory.CreateCandles(5, initId: true);
    candleRep.Table.Returns(expected_arr.BuildMock());
    // Act
    var asserted_res = srv.GetAllAsync().Result;

    // Assert
    Assert.Equal(expected_arr.Count(), asserted_res.Count());
    foreach (var expected_el in expected_arr)
    {
      Assert.Contains(expected_el.ToDto(), asserted_res);
    }
  }

}