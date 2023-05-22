using System.Net;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.Tool;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Test.TimeFrameAggregate.Service.ITimeFrameSrvTest;

public class GetAll_Test : BaseTest
{

  public GetAll_Test(ITestOutputHelper output) : base(nameof(GetAll_Test), output)
  {

  }

  [Fact]
  public void WHEN_get_all_timeframe_THEN_get_all_timeframes()
  {
    // Array
    using var scope = serviceProvider.CreateScope();
    var srv = scope.ServiceProvider.GetService<ITimeFrameSrv>();

    // Act
    var asserted_arr = srv.GetAllAsync().Result;

    // Assert
    foreach (var enumId in TimeFrame.ToList().Select(e => e.EnumId))
    {
      Assert.Contains(new TimeFrameResponseDto(new TimeFrame(enumId)), asserted_arr);
    }
  }
}