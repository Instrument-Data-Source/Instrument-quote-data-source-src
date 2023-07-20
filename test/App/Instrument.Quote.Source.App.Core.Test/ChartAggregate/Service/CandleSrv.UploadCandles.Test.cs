using System.Net;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using MockQueryable.Moq;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.ChartAggregate.Service;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Microsoft.EntityFrameworkCore.Storage;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Service;
public class CandleSrv_AddCandles_Test : BaseTest<CandleSrv_AddCandles_Test>
{
  CandlesSrv assertedSrv;
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private TimeFrame usedTf = TimeFrame.Enum.D1.ToEntity();
  private IReadRepository<ent.Instrument> instrumentRep = Substitute.For<IReadRepository<ent.Instrument>>();
  private IReadRepository<TimeFrame> timeframeRep = Substitute.For<IReadRepository<TimeFrame>>();
  private IRepository<Chart> chartRep = Substitute.For<IRepository<Chart>>();
  private IReadRepository<Candle> candleRep = Substitute.For<IReadRepository<Candle>>();
  public CandleSrv_AddCandles_Test(ITestOutputHelper output) : base(output)
  {
    assertedSrv = new CandlesSrv(chartRep, instrumentRep, timeframeRep, candleRep, output.BuildLoggerFor<CandlesSrv>());
    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    timeframeRep.Table.Returns(new[] { usedTf }.BuildMock());
    chartRep.Table.Returns(new Chart[] { }.BuildMock());
  }

  [Fact]
  public async Task WHEN_add_for_empty_instrument_THEN_add_all_candles()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var expectedFrom = new DateTime(2020, 3, 1).ToUniversalTime();
    var expectedUntill = new DateTime(2020, 4, 1).ToUniversalTime();
    var expectedChart = new MockChartFactory(mockInstrument, usedTf).CreateChart();
    var expectedCandles = new MockCandleFactory(expectedChart).CreateCandleDtos(expectedFrom, expectedUntill);
    var usedMapper = new CandleMapper(expectedChart);
    var usedUploadDto = new UploadedCandlesDto()
    {
      FromDate = expectedFrom,
      UntillDate = expectedUntill,
      Candles = expectedCandles
    };
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.AddCandlesAsync(mockInstrument.Id, usedTf.Id, usedUploadDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is sucess", () => Assert.True(assertedResult.IsSuccess));

    Expect("Result is contain count of added candles", () => Assert.Equal(expectedCandles.Count(), assertedResult.Value));

    Expect("LoadedPeriod Repository was called to save new period", () =>
      chartRep.Received().AddAsync(
        Arg.Is<Chart>(e =>
          e.FromDate == expectedFrom &&
          e.UntillDate == expectedUntill &&
          e.InstrumentId == mockInstrument.Id &&
          e.TimeFrameId == usedTf.Id &&
          e.Candles.Count() == expectedCandles.Count() &&
          e.Candles.Select(usedMapper.map).All(c => expectedCandles.Contains(c))),
        Arg.Any<IDbContextTransaction>(),
        Arg.Any<CancellationToken>()
      ).Wait()
    );
    #endregion
  }

  [Fact]
  public async void WHEN_instrument_not_found_THEN_exception()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var usedFromDate = new DateTime(2020, 3, 1).ToUniversalTime();
    var usedUntillDate = new DateTime(2020, 4, 1).ToUniversalTime();
    var expectedCandles = new MockCandleFactory().CreateCandleDtos(usedFromDate, usedUntillDate);
    var usedInstrumentId = MockInstrument.Create().Id;

    var usedUploadDto = new UploadedCandlesDto()
    {
      FromDate = usedFromDate,
      UntillDate = usedUntillDate,
      Candles = expectedCandles
    };
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    var assertedResult = await assertedSrv.AddCandlesAsync(usedInstrumentId, usedTf.Id, usedUploadDto);
    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Errors is correct", () =>
    {
      Expect("Error count is 1", () => Assert.Equal(1, assertedResult.Errors.Count()));
      Expect("Error is Instrument", () => Assert.True(assertedResult.Errors.Contains(nameof(ent.Instrument))));
    });

    #endregion
  }

  [Fact]
  public async Task WHEN_timeframe_not_found_THEN_exception()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var usedFromDate = new DateTime(2020, 3, 1).ToUniversalTime();
    var usedUntillDate = new DateTime(2020, 4, 1).ToUniversalTime();
    var expectedCandles = new MockCandleFactory().CreateCandleDtos(usedFromDate, usedUntillDate);

    var usedUploadDto = new UploadedCandlesDto()
    {
      FromDate = usedFromDate,
      UntillDate = usedUntillDate,
      Candles = expectedCandles
    };
    #endregion

    #region Act
    this.logger.LogDebug("Test ACT");
    var assertedResult = await assertedSrv.AddCandlesAsync(mockInstrument.Id, 99, usedUploadDto);
    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Errors is correct", () =>
    {
      Expect("Error count is 1", () => Assert.Equal(1, assertedResult.Errors.Count()));
      Expect("Error is TimeFrame", () => Assert.True(assertedResult.Errors.Contains(nameof(TimeFrame))));
    });

    #endregion
  }

  [Fact]
  public async void WHEN_candleDto_invalid_THEN_exception()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var usedFromDate = new DateTime(2020, 3, 1).ToUniversalTime();
    var usedUntillDate = new DateTime(2020, 4, 1).ToUniversalTime();
    var expectedCandles = new MockCandleFactory().CreateCandleDtos(usedFromDate, usedUntillDate);
    var usedUploadDto = new UploadedCandlesDto()
    {
      FromDate = usedFromDate,
      UntillDate = usedUntillDate,
      Candles = expectedCandles
    };
    expectedCandles.ElementAt(2).Open = 2.3434435m;
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");


    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Throw exception", () => Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await assertedSrv.AddCandlesAsync(mockInstrument.Id, usedTf.Id, usedUploadDto)).GetAwaiter().GetResult(), out var assertedException);

    #endregion
  }


}

public class CandleSrv_AddCandles_For_ExistData_Test : BaseTest<CandleSrv_AddCandles_For_ExistData_Test>
{
  CandlesSrv assertedSrv;
  private ent.Instrument mockInstrument;
  private TimeFrame usedTimeFrame;
  private IReadRepository<ent.Instrument> instrumentRep = Substitute.For<IReadRepository<ent.Instrument>>();
  private IReadRepository<TimeFrame> timeframeRep = Substitute.For<IReadRepository<TimeFrame>>();
  private IRepository<Chart> chartRep = Substitute.For<IRepository<Chart>>();
  private IReadRepository<Candle> candleRep = Substitute.For<IReadRepository<Candle>>();
  private Chart usedChart;
  private MockChartFactory mockChartFactory;
  private MockCandleFactory mockCandleFactory;
  private IEnumerable<Candle> baseCandles;
  public CandleSrv_AddCandles_For_ExistData_Test(ITestOutputHelper output) : base(output)
  {
    assertedSrv = new CandlesSrv(chartRep, instrumentRep, timeframeRep, candleRep, output.BuildLoggerFor<CandlesSrv>());

    mockChartFactory = new MockChartFactory();
    mockInstrument = mockChartFactory.instrument;
    usedTimeFrame = mockChartFactory.timeFrame;

    usedChart = mockChartFactory.CreateChart(initId: true);

    mockCandleFactory = new MockCandleFactory(usedChart);
    baseCandles = mockCandleFactory.CreateCandles(usedChart.FromDate, usedChart.UntillDate);
    usedChart.AddCandles(baseCandles);

    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    timeframeRep.Table.Returns(new[] { usedTimeFrame }.BuildMock());
    chartRep.Table.Returns(new Chart[] { usedChart }.BuildMock());
  }

  [Fact]
  public async void WHEN_new_period_is_joined_THEN_add_new_data()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expectedUntilDate = usedChart.UntillDate;
    var expectedeFromDate = usedChart.FromDate - new TimeSpan(10, 0, 0, 0);
    var usedUntillDate = usedChart.FromDate;
    (var expectedCandles, var dtos) = mockCandleFactory.CreateCandlesAndDtos(expectedeFromDate, usedUntillDate);
    var usedUploadDto = new UploadedCandlesDto()
    {
      FromDate = expectedeFromDate,
      UntillDate = usedUntillDate,
      Candles = dtos
    };
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.AddCandlesAsync(mockInstrument.Id, usedTimeFrame.Id, usedUploadDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is sucess", () => Assert.True(assertedResult.IsSuccess));

    Expect("Result is contain count of added candles", () => Assert.Equal(expectedCandles.Count(), assertedResult.Value));

    ExpectGroup("Period has extended dates", () =>
    {
      Expect("From date is extend correctly", () => Assert.Equal(expectedeFromDate, usedChart.FromDate));
      Expect("Untill date is extend correctly", () => Assert.Equal(expectedUntilDate, usedChart.UntillDate));
    });


    ExpectGroup("Candles exptend correctly", () =>
    {
      Expect("Count is summed", () => Assert.Equal(baseCandles.Count() + expectedCandles.Count(), usedChart.Candles.Count()));
      Expect("Candles contain base candles", () => Assert.True(baseCandles.All(c => usedChart.Candles.Contains(c))));
      Expect("Candles contain new candles", () => Assert.True(expectedCandles.Select(c => c.DateTime).All(c => usedChart.Candles.Select(c => c.DateTime).Contains(c))));
    });

    Expect("LoadedPeriod Repository was called to save new period", () =>
      chartRep.Received().SaveChangesAsync(Arg.Any<CancellationToken>()).Wait()
    );
    #endregion
  }

  [Fact]
  public async void WHEN_new_period_not_joined_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var usedFromDate = usedChart.FromDate - new TimeSpan(20, 0, 0, 0);
    var usedUntillDate = usedChart.FromDate - new TimeSpan(2, 0, 0, 0);
    var expectedCandles = new MockCandleFactory().CreateCandleDtos(usedFromDate, usedUntillDate);
    var usedUploadDto = new UploadedCandlesDto()
    {
      FromDate = usedFromDate,
      UntillDate = usedUntillDate,
      Candles = expectedCandles
    };
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.AddCandlesAsync(mockInstrument.Id, usedTimeFrame.Id, usedUploadDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is sucess", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result is Invalid", () => Assert.Equal(ResultStatus.Error, assertedResult.Status));
    Expect("Result has one validation error", () => Assert.Single(assertedResult.Errors), out var assertedError);
    #endregion
  }

  [Fact]
  public async void WHEN_new_period_cross_with_exist_data_THEN_error()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var usedFromDate = usedChart.FromDate - new TimeSpan(20, 0, 0, 0);
    var usedUntillDate = usedChart.FromDate + new TimeSpan(2, 0, 0, 0);
    var expectedCandles = new MockCandleFactory().CreateCandleDtos(usedFromDate, usedUntillDate);
    var usedUploadDto = new UploadedCandlesDto()
    {
      FromDate = usedFromDate,
      UntillDate = usedUntillDate,
      Candles = expectedCandles
    };
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.AddCandlesAsync(mockInstrument.Id, usedTimeFrame.Id, usedUploadDto);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is sucess", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is Conflict", () => Assert.Equal(ResultStatus.Conflict, assertedResult.Status));
    #endregion
  }


}