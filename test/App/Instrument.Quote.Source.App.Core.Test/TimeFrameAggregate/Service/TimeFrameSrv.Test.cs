using System.Net;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Service;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.TimeFrameAggregate.Service;


public class TimeFrameSrv_Test
{
  private readonly ITestOutputHelper output;
  private ITimeFrameSrv timeFrameSrv;
  private IReadRepository<TimeFrame> tfRep = Substitute.For<IReadRepository<TimeFrame>>();

  public TimeFrameSrv_Test(ITestOutputHelper output)
  {
    this.output = output;
    timeFrameSrv = new TimeFrameSrv(tfRep);
  }


  [Fact]
  public void WHEN_request_all_THEN_get_all_from_repository()
  {
    // Array
    var expected_arr = new[] { new TimeFrame(TimeFrame.Enum.M), new TimeFrame(TimeFrame.Enum.W1) };
    tfRep.Table.Returns(expected_arr.BuildMock());

    // Act
    var asserted_res = timeFrameSrv.GetAllAsync().Result;

    // Assert
    Assert.True(asserted_res.IsSuccess);
    Assert.Equal(expected_arr.Count(), asserted_res.Value.Count());
    foreach (var expected_el in expected_arr)
    {
      Assert.Contains(new TimeFrameResponseDto(expected_el), asserted_res.Value);
    }
  }
}