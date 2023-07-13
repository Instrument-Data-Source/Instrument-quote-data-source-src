using Ardalis.Result;
using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Service;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.Mock;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Exceptions;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Service.CandleSrvRefTest;
public class Add_Test : BaseTest<Add_Test>
{
  CandleSrvRef srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private TimeFrame usedTf = TimeFrame.Enum.D1.ToEntity();
  private MockCandleFactory candleFactory;
  public Add_Test(ITestOutputHelper output) : base(output)
  {
    srv = new CandleSrvRef(instrumentRep, timeframeRep, loadedPeriodRep, candleRep);

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
    var expectedDto = new NewPeriodDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      InstrumentId = mockInstrument.Id,
      TimeFrameId = usedTf.Id,
      Candles = expectedCandleDtos
    };

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await srv.AddAsync(expectedDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Success result", () => Assert.True(assertedResult.IsSuccess));

    Expect("Result is contain count of added candles", () => Assert.Equal(expectedCandleDtos.Count(), assertedResult.Value));

    Expect("LoadedPeriod Repository was called to save new period", () =>
      loadedPeriodRep.Received().AddAsync(
        Arg.Is<LoadedPeriod>(e =>
          e.FromDate == expectedFrom &&
          e.UntillDate == expectedUntill &&
          e.InstrumentId == expectedDto.InstrumentId &&
          e.TimeFrameId == expectedDto.TimeFrameId &&
          e.Candles.Count() == expectedCandleDtos.Count() &&
          e.Candles.All(c => expectedCandleDtos.Contains(c.ToDto()))),
        Arg.Any<IDbContextTransaction>(),
        Arg.Any<CancellationToken>()
      ).Wait()
    );

    #endregion
  }
}

public class Add_invalid_dto_Test : BaseTest<Add_invalid_dto_Test>
{
  CandleSrvRef srv;
  private IRepository<Candle> candleRep = Substitute.For<IRepository<Candle>>();
  private IRepository<LoadedPeriod> loadedPeriodRep = Substitute.For<IRepository<LoadedPeriod>>();
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private TimeFrame usedTf = TimeFrame.Enum.D1.ToEntity();
  private MockCandleFactory candleFactory;
  public Add_invalid_dto_Test(ITestOutputHelper output) : base(output)
  {
    srv = new CandleSrvRef(instrumentRep, timeframeRep, loadedPeriodRep, candleRe);

    candleFactory = new MockCandleFactory(mockInstrument, usedTf);
    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    timeframeRep.Table.Returns(new[] { usedTf }.BuildMock());
  }

  [Fact]
  public async Task WHEN_incorect_dto_THEN_return_invalid_resultAsync()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedFrom = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntill = new DateTime(2010, 1, 10).ToUniversalTime();
    var expectedCandleDtos = candleFactory.CreateCandles(expectedFrom, expectedUntill).Select(e => e.ToDto());

    var expectedDto = new NewPeriodDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      InstrumentId = mockInstrument.Id,
      TimeFrameId = usedTf.Id,
      Candles = expectedCandleDtos
    };

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    Expect("Get validation Exception", () => Assert.ThrowsAsync<ValidationException>(async () => await srv.AddAsync(expectedDto)).Result, out var assertedExpection);
    this.logger.LogInformation("Get exception:\n" + assertedExpection.ToString());
    #endregion
  }

  public static IEnumerable<object[]> IncorrectId
  {
    get
    {

      yield return new object[] { MockInstrument.Create().Id, 999 };
      yield return new object[] { MockInstrument.Create().Id, null };
      yield return new object[] { null, 999 };
    }
  }

  [Theory]
  [MemberData(nameof(IncorrectId))]
  public async Task WHEN_incorect_related_entity_id_not_found_THEN_return_notfound_resultAsync(int? instrumentId, int? timeframeId)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedFrom = new DateTime(2020, 1, 1).ToUniversalTime();
    var expectedUntill = new DateTime(2020, 1, 10).ToUniversalTime();
    var expectedCandleDtos = candleFactory.CreateCandles(expectedFrom, expectedUntill).Select(e => e.ToDto());

    var expectedDto = new NewPeriodDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      InstrumentId = instrumentId ?? mockInstrument.Id,
      TimeFrameId = timeframeId ?? usedTf.Id,
      Candles = expectedCandleDtos
    };

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    Expect("Get IdNotFoundException",
      () => Assert.ThrowsAsync<IdNotFoundException>(async () => await srv.AddAsync(expectedDto)).Result,
      out var assertedException);
    this.logger.LogInformation("Get exception:\n" + assertedException.ToString());

    #endregion
  }
}

public class Add_with_exist_period_Test : BaseTest<Add_with_exist_period_Test>
{
  CandleSrvRef srv;
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
    srv = new CandleSrvRef(instrumentRep, timeframeRep, loadedPeriodRep, candleRep);

    candleFactory = new MockCandleFactory(mockInstrument, usedTf);
    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    timeframeRep.Table.Returns(new[] { usedTf }.BuildMock());

    periodFactory = new MockPeriodFactory(mockInstrument, usedTf);

    mockPeriod = periodFactory.CreatePeriod(new DateTime(2020, 1, 1), new DateTime(2020, 3, 1));
    loadedPeriodRep.Table.Returns(new[] { mockPeriod }.BuildMock());
  }

  [Fact]
  public async void WHEN_add_joined_period_THEN_period_added()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expectedUntillDt = mockPeriod.UntillDate;
    var usingFromDt = new DateTime(2019, 11, 1).ToUniversalTime();
    var usingUntillDt = new DateTime(2020, 1, 1).ToUniversalTime();
    var usingCandleDtos = candleFactory.CreateCandles(usingFromDt, usingUntillDt).Select(e => e.ToDto());
    var usingAddCandelDto = new NewPeriodDto()
    {
      FromDate = usingFromDt,
      UntillDate = usingUntillDt,
      InstrumentId = mockInstrument.Id,
      TimeFrameId = usedTf.Id,
      Candles = usingCandleDtos
    };

    var expectedCandleCount = mockPeriod.Candles.Count() + usingCandleDtos.Count();

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await srv.AddAsync(usingAddCandelDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Method return count of new candles", () => Assert.Equal(usingCandleDtos.Count(), assertedResult.Value));

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
  public async Task WHEN_add_not_joined_period_THEN_errorAsync()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expectedUntillDt = mockPeriod.UntillDate;
    var usingFromDt = new DateTime(2019, 11, 1).ToUniversalTime();
    var usingUntillDt = new DateTime(2019, 11, 20).ToUniversalTime();
    var usingCandleDtos = candleFactory.CreateCandles(usingFromDt, usingUntillDt).Select(e => e.ToDto());
    var usingAddCandelDto = new NewPeriodDto()
    {
      FromDate = usingFromDt,
      UntillDate = usingUntillDt,
      InstrumentId = mockInstrument.Id,
      TimeFrameId = usedTf.Id,
      Candles = usingCandleDtos
    };

    var expectedCandleCount = mockPeriod.Candles.Count() + usingCandleDtos.Count();

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await srv.AddAsync(usingAddCandelDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is Error", () => Assert.Equal(ResultStatus.Error, assertedResult.Status));
    Expect("Result contain one error", () => Assert.Single(assertedResult.Errors), out var assertedError);
    Expect("Result error text", () => Assert.Equal("Period not joined", assertedError));
    #endregion
  }

  [Fact]
  public async Task WHEN_add_crossed_period_THEN_errorAsync()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expectedUntillDt = mockPeriod.UntillDate;
    var usingFromDt = new DateTime(2019, 11, 1).ToUniversalTime();
    var usingUntillDt = new DateTime(2020, 1, 20).ToUniversalTime();
    var usingCandleDtos = candleFactory.CreateCandles(usingFromDt, usingUntillDt).Select(e => e.ToDto());
    var usingAddCandelDto = new NewPeriodDto()
    {
      FromDate = usingFromDt,
      UntillDate = usingUntillDt,
      InstrumentId = mockInstrument.Id,
      TimeFrameId = usedTf.Id,
      Candles = usingCandleDtos
    };

    var expectedCandleCount = mockPeriod.Candles.Count() + usingCandleDtos.Count();

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await srv.AddAsync(usingAddCandelDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is Error", () => Assert.Equal(ResultStatus.Error, assertedResult.Status));
    Expect("Result contain one error", () => Assert.Single(assertedResult.Errors), out var assertedError);
    Expect("Result error text", () => Assert.Equal("Period not joined", assertedError));
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

      var assertedResult = await srv.AddAsync(usingAddCandelDto);

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

      var assertedResult = await srv.AddAsync(usingAddCandelDto);

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
