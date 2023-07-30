using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Dto;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Service;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Exceptions;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
using MediatR;

namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Service;

public class CandleSrv_GetCandles_Test : BaseTest<CandleSrv_GetCandles_Test>
{
  CandlesSrv assertedSrv;
  private ent.Instrument mockInstrument;
  private TimeFrame usedTimeFrame;
  private TimeFrame usedTimeFrame2;
  private IReadRepository<ent.Instrument> instrumentRep = Substitute.For<IReadRepository<ent.Instrument>>();
  private IReadRepository<TimeFrame> timeframeRep = Substitute.For<IReadRepository<TimeFrame>>();
  private IRepository<Chart> chartRep = Substitute.For<IRepository<Chart>>();
  private IReadRepository<Candle> candleRep = Substitute.For<IReadRepository<Candle>>();
  private IMediator mediator = Substitute.For<IMediator>();
  private Chart usedChart;
  private MockChartFactory mockChartFactory;
  private MockCandleFactory mockCandleFactory;
  private IEnumerable<Candle> baseCandles;
  private IEnumerable<CandleDto> baseDtos;
  public CandleSrv_GetCandles_Test(ITestOutputHelper output) : base(output)
  {
    assertedSrv = new CandlesSrv(chartRep, instrumentRep, timeframeRep, candleRep, mediator, output.BuildLoggerFor<CandlesSrv>());

    mockChartFactory = new MockChartFactory();
    mockInstrument = mockChartFactory.instrument;
    usedTimeFrame = mockChartFactory.timeFrame;
    usedTimeFrame2 = TimeFrame.Enum.M.ToEntity();
    if (usedTimeFrame.Id == usedTimeFrame2.Id)
      throw new Exception("Id (enum) must be different");

    usedChart = mockChartFactory.CreateChart(initId: true);

    mockCandleFactory = new MockCandleFactory(usedChart);
    (baseCandles, baseDtos) = mockCandleFactory.CreateCandlesAndDtos(usedChart.FromDate, usedChart.UntillDate);
    usedChart.AddCandles(baseCandles);

    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    timeframeRep.Table.Returns(new[] { usedTimeFrame, usedTimeFrame2 }.BuildMock());
    chartRep.Table.Returns(new Chart[] { usedChart }.BuildMock());
    candleRep.Table.Returns(baseCandles.BuildMock());
  }

  [Fact]
  public async void WHEN_request_exist_data_THEN_get_it()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    DateTime usedDtFrom = usedChart.FromDate;
    DateTime usedDtUntill = usedChart.UntillDate;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetAsync(mockInstrument.Id, usedTimeFrame.Id, usedDtFrom, usedDtUntill);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is Success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Candles load correctly", () =>
     {
       Expect("Count is correct", () => Assert.Equal(baseCandles.Count(), assertedResult.Value.Count()));
       Expect("Result contain all base candles", () => Assert.True(baseDtos.All(c => assertedResult.Value.Contains(c))));
     });
    #endregion
  }

  [Fact]
  public async void WHEN_request_unknown_instrument_THEN_return_NotFound()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    DateTime usedDtFrom = usedChart.FromDate;
    DateTime usedDtUntill = usedChart.UntillDate;
    var usedInstrumentId = mockChartFactory.instrument.Id + 1;
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetAsync(usedInstrumentId, usedTimeFrame.Id, usedDtFrom, usedDtUntill);

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
  public async void WHEN_request_unknown_timeframe_THEN_return_NotFound()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    DateTime usedDtFrom = usedChart.FromDate;
    DateTime usedDtUntill = usedChart.UntillDate;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    var assertedResult = await assertedSrv.GetAsync(mockInstrument.Id, 99, usedDtFrom, usedDtUntill);
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
  public async void WHEN_request_unloaded_chart_THEN_return_NotFound()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    DateTime usedDtFrom = usedChart.FromDate;
    DateTime usedDtUntill = usedChart.UntillDate;
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetAsync(mockInstrument.Id, usedTimeFrame2.Id, usedDtFrom, usedDtUntill);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is Success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    Expect("Result errors contain 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
    Expect("Error is Chart", () => Assert.True(assertedResult.Errors.Contains(nameof(Chart))));
    #endregion
  }

  [Fact]
  public async void WHEN_request_unloaded_data_THEN_return_NotFound()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    DateTime usedDtFrom = usedChart.FromDate;
    DateTime usedDtUntill = usedChart.UntillDate + new TimeSpan(1, 0, 0, 0);

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetAsync(mockInstrument.Id, usedTimeFrame.Id, usedDtFrom, usedDtUntill);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is Success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    Expect("Result errors contain 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
    Expect("Error is Candle", () => Assert.True(assertedResult.Errors.Contains(nameof(Candle))));
    #endregion
  }
}