using System.Net;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.ChartAggregate.Interface;
using Instrument.Quote.Source.App.Core.ChartAggregate.Mapper;
using Instrument.Quote.Source.App.Core.ChartAggregate.Model;
using Instrument.Quote.Source.App.Core.ChartAggregate.Service;
using Instrument.Quote.Source.App.Core.Test.ChartAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.ChartAggregate.Service;


public class ChartSrv_GetAllLoadedPeriods_Test : BaseTest<ChartSrv_GetAllLoadedPeriods_Test>
{
  IChartSrv assertedSrv;
  IReadRepository<Chart> usedChartRep = Substitute.For<IReadRepository<Chart>>();
  IReadRepository<ent.Instrument> instrumentRep = Substitute.For<IReadRepository<ent.Instrument>>();
  MockChartFactory mockChartFactory1;
  Chart mockChart1_1;
  MockChartFactory mockChartFactory2;
  Chart mockChart2_1;
  Chart mockChart2_2;
  public ChartSrv_GetAllLoadedPeriods_Test(ITestOutputHelper output) : base(output)
  {
    assertedSrv = new ChartSrv(usedChartRep, instrumentRep, output.BuildLoggerFor<ChartSrv>());

    mockChartFactory1 = new MockChartFactory();
    mockChart1_1 = mockChartFactory1.CreateChart();

    mockChartFactory2 = new MockChartFactory(TimeFrame.Enum.H1);
    mockChart2_1 = mockChartFactory2.CreateChart();
    mockChart2_2 = mockChartFactory2.CreateChart();


  }

  [Fact]
  public async Task WHEN_periods_exist_THEN_return_data()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    usedChartRep.Table.Returns(new[] { mockChart1_1, mockChart2_1, mockChart2_2 }.BuildMock());

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetAllLoadedPeriodsAsync();

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Result contaian correct data", () =>
    {
      Expect("Count of charts is correct", () => Assert.Equal(3, assertedResult.Value.Count()));
      Expect("Chart 1 1 is exsist and correct", () => Assert.True(assertedResult.Value.ToList().Contains(mockChart1_1.ToDto())));
      Expect("Chart 2 1 is exsist and correct", () => Assert.True(assertedResult.Value.ToList().Contains(mockChart2_1.ToDto())));
      Expect("Chart 2 2 is exsist and correct", () => Assert.True(assertedResult.Value.ToList().Contains(mockChart2_2.ToDto())));
    });

    #endregion
  }


  [Fact]
  public async void WHEN_periods_not_exist_THEN_return_empty_list()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    usedChartRep.Table.Returns(new Chart[] { }.BuildMock());

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var assertedResult = await assertedSrv.GetAllLoadedPeriodsAsync();

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contaian empty data", () => Assert.Empty(assertedResult.Value));

    #endregion
  }

}