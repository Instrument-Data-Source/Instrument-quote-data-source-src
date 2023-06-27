using Ardalis.Result;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Service.CandleSrvTest;
public class Add_Test : BaseTest<Add_Test>
{
  ICandleSrv srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private TimeFrame usedTf = TimeFrame.Enum.D1.ToEntity();
  private MockCandleFactory candleFactory;
  public Add_Test(ITestOutputHelper output) : base(output)
  {
    srv = new CandleSrv(candleRep, loadedPeriodRep, instrumentRep, timeframeRep);

    candleFactory = new MockCandleFactory(mockInstrument, usedTf);
    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    timeframeRep.Table.Returns(new[] { usedTf }.BuildMock());
  }

  [Fact]
  public async Task WHEN_add_for_empty_instrument_THEN_add_all_candlesAsync()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedFrom = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntill = new DateTime(2020, 1, 10).ToUniversalTime();
    var expectedCandleDtos = candleFactory.CreateCandles(expectedFrom, expectedUntill).Select(e => e.ToDto());

    loadedPeriodRep.Table.Returns(new List<LoadedPeriod>().BuildMock());
    var expectedDto = new AddCandlesDto()
    {
      From = expectedFrom,
      Untill = expectedUntill,
      instrumentId = mockInstrument.Id,
      timeFrameId = usedTf.Id,
      Candles = expectedCandleDtos
    };

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await srv.AddAsync(expectedDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));

    Expect("Result is correct", () => Assert.Equal(expectedCandleDtos.Count(), assertedResult.Value));

    Expect("LoadedPeriod Repository was called to save new period", () =>
      loadedPeriodRep.Received().AddAsync(
        Arg.Is<LoadedPeriod>(e =>
          e.FromDate == expectedFrom &&
          e.UntillDate == expectedUntill &&
          e.InstrumentId == expectedDto.instrumentId &&
          e.TimeFrameId == expectedDto.timeFrameId &&
          e.Candles.Count() == expectedCandleDtos.Count() &&
          e.Candles.All(c => expectedCandleDtos.Contains(c.ToDto()))),
        Arg.Any<IDbContextTransaction>(),
        Arg.Any<CancellationToken>()
      ).Wait()
    );

    #endregion
  }

}

public class Add_with_exist_period_Test : BaseTest<Add_with_exist_period_Test>
{
  ICandleSrv srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private TimeFrame usedTf = TimeFrame.Enum.D1.ToEntity();
  private LoadedPeriod mockPeriod;
  private MockCandleFactory candleFactory;
  private MockPeriodFactory periodFactory;
  public Add_with_exist_period_Test(ITestOutputHelper output) : base(output)
  {
    srv = new CandleSrv(candleRep, loadedPeriodRep, instrumentRep, timeframeRep);

    candleFactory = new MockCandleFactory(mockInstrument, usedTf);
    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    timeframeRep.Table.Returns(new[] { usedTf }.BuildMock());

    periodFactory = new MockPeriodFactory(mockInstrument, usedTf);

    mockPeriod = periodFactory.CreatePeriod(new DateTime(2020, 1, 1), new DateTime(2020, 3, 1));
    loadedPeriodRep.Table.Returns(new[] { mockPeriod }.BuildMock());
  }

  [Fact]
  public void WHEN_add_addition_period_THEN_preriod_joined()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expectedUntillDt = mockPeriod.UntillDate;
    var usingFromDt = new DateTime(2019, 11, 1).ToUniversalTime();
    var usingUntillDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var usingCandleDtos = candleFactory.CreateCandles(usingFromDt, usingUntillDt).Select(e => e.ToDto());
    var usingAddCandelDto = new AddCandlesDto()
    {
      From = usingFromDt,
      Untill = usingUntillDt,
      instrumentId = mockInstrument.Id,
      timeFrameId = usedTf.Id,
      Candles = usingCandleDtos
    };

    var expectedCandleCount = mockPeriod.Candles.Count() + usingCandleDtos.Count();

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = srv.AddAsync(usingAddCandelDto).Result;

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));

    Expect("Result is correct", () => Assert.Equal(usingCandleDtos.Count(), assertedResult.Value));

    ExpectGroup("Exist pretiod changed", () =>
    {
      Expect("From datetime changed", () => Assert.Equal(usingFromDt, mockPeriod.FromDate));
      Expect("Until datetime does not changed", () => Assert.Equal(expectedUntillDt, mockPeriod.UntillDate));
      Expect("Candles has joined count", () => Assert.Equal(expectedCandleCount, mockPeriod.Candles.Count()));
    });

    Expect("Called method to savechanges", async () =>
    {
      await loadedPeriodRep.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
    });
    #endregion
  }
  /*
    [Fact]
    public void WHEN_add_cross_period_THEN_add_only_new_data()
    {
      #region Array
      this.logger.LogDebug("Test ARRAY");

      this.logger.LogDebug("Test ARRAY");
      var expectedUntillDt = mockPeriod.UntillDate;
      var usingFromDt = new DateTime(2019, 11, 1).ToUniversalTime();
      var usingUntillDt = new DateTime(2020, 1, 10).ToUniversalTime();
      var usingNewCandleDtos = candleFactory.CreateCandles(usingFromDt, mockPeriod.FromDate).Select(e => e.ToDto());

      var usingCandlesDto = new List<CandleDto>();
      usingCandlesDto.AddRange(usingNewCandleDtos);
      usingCandlesDto.AddRange(mockPeriod.Candles.Where(c => c.DateTime < usingUntillDt).Select(c => c.ToDto()));

      var usingAddCandelDto = new AddCandlesDto()
      {
        From = usingFromDt,
        Untill = usingUntillDt,
        instrumentId = mockInstrument.Id,
        timeFrameId = usedTf.Id,
        Candles = usingCandlesDto
      };

      var expectedCandleCount = mockPeriod.Candles.Count() + usingNewCandleDtos.Count();

      #endregion


      #region Act
      this.logger.LogDebug("Test ACT");

      var assertedResult = srv.AddAsync(usingAddCandelDto).Result;

      #endregion


      #region Assert
      this.logger.LogDebug("Test ASSERT");

      Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));

      Expect("Result is correct", () => Assert.Equal(usingNewCandleDtos.Count(), assertedResult.Value));

      ExpectGroup("Exist pretiod changed", () =>
      {
        Expect("From datetime changed", () => Assert.Equal(usingFromDt, mockPeriod.FromDate));
        Expect("Until datetime does not changed", () => Assert.Equal(expectedUntillDt, mockPeriod.UntillDate));
        Expect("Candles has joined count", () => Assert.Equal(expectedCandleCount, mockPeriod.Candles.Count()));
      });

      Expect("Called method to savechanges", async () =>
      {
        await loadedPeriodRep.Received().SaveChangesAsync(Arg.Any<CancellationToken>());
      });

      #endregion
    }

    [Fact]
    public void WHEN_add_cross_period_with_different_candles_THEN_return_validation_errors()
    {
      #region Array
      this.logger.LogDebug("Test ARRAY");

      this.logger.LogDebug("Test ARRAY");
      var usingFromDt = new DateTime(2019, 11, 1).ToUniversalTime();
      var usingUntillDt = new DateTime(2020, 1, 10).ToUniversalTime();
      var usingNewCandleDtos = candleFactory.CreateCandles(usingFromDt, usingUntillDt).Select(e => e.ToDto());

      var usingAddCandelDto = new AddCandlesDto()
      {
        From = usingFromDt,
        Untill = usingUntillDt,
        instrumentId = mockInstrument.Id,
        timeFrameId = usedTf.Id,
        Candles = usingNewCandleDtos
      };

      #endregion


      #region Act
      this.logger.LogDebug("Test ACT");

      var assertedResult = srv.AddAsync(usingAddCandelDto).Result;

      #endregion


      #region Assert
      this.logger.LogDebug("Test ASSERT");

      Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));

      Expect("Result status is Invalid", () => Assert.Equal(ResultStatus.Invalid, assertedResult.Status));

      Expect("Result contain validations errors", () => Assert.NotEmpty(assertedResult.ValidationErrors));

      #endregion
    }
    */
}
