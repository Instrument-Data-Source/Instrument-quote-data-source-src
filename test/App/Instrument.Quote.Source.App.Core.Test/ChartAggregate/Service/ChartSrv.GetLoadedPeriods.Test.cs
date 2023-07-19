using System.Net;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Service;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Exceptions;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Service;


public class ChartSrv_GetLoadedPeriods_Test : BaseTest<ChartSrv_GetLoadedPeriods_Test>
{
  IChartSrv assertedSrv;
  IReadRepository<Chart> usedChartRep = Substitute.For<IReadRepository<Chart>>();
  IReadRepository<ent.Instrument> usedInstrumuntRep = Substitute.For<IReadRepository<ent.Instrument>>();
  MockChartFactory mockChartFactory1;
  Chart mockChart1_1;
  MockChartFactory mockChartFactory2;
  ent.Instrument mockInstrument3 = MockInstrument.Create();
  Chart mockChart2_1;
  Chart mockChart2_2;
  public ChartSrv_GetLoadedPeriods_Test(ITestOutputHelper output) : base(output)
  {
    assertedSrv = new ChartSrv(usedChartRep, usedInstrumuntRep, output.BuildLoggerFor<ChartSrv>());

    mockChartFactory1 = new MockChartFactory();
    mockChart1_1 = mockChartFactory1.CreateChart();

    mockChartFactory2 = new MockChartFactory(TimeFrame.Enum.H1);
    mockChart2_1 = mockChartFactory2.CreateChart();
    mockChart2_2 = mockChartFactory2.CreateChart();

    usedChartRep.Table.Returns(new[] { mockChart1_1, mockChart2_1, mockChart2_2 }.BuildMock());
    usedInstrumuntRep.Table.Returns(new[] { mockChartFactory1.instrument, mockChartFactory2.instrument, mockInstrument3 }.BuildMock());
  }

  [Fact]
  public async void WHEN_periods_exist_THEN_return_data()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetLoadedPeriodsAsync(mockChart2_2.InstrumentId);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Result contaian correct data", () =>
    {
      Expect("Count of charts is correct", () => Assert.Equal(2, assertedResult.Value.Count()));
      Expect("Chart 2 1 is exsist and correct", () =>
        Assert.True(assertedResult.Value.ToList().Contains(mockChart2_1.ToDto())));
      Expect("Chart 2 2 is exsist and correct", () =>
        Assert.True(assertedResult.Value.ToList().Contains(mockChart2_2.ToDto())));
    });

    #endregion
  }



  [Fact]
  public async void WHEN_periods_not_exist_THEN_return_empty_list()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetLoadedPeriodsAsync(mockInstrument3.Id);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contaian empty data", () => Assert.Empty(assertedResult.Value));

    #endregion
  }

  [Fact]
  public async void WHEN_instrument_does_not_exist_THEN_return_not_found()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var usedId = MockInstrument.Create().Id;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetLoadedPeriodsAsync(usedId);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");
    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Errors is correct", () =>
    {
      Expect("Only 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error contain Instrument Entity", () => Assert.Equal(nameof(ent.Instrument), assertedError));
    });
    #endregion
  }

}