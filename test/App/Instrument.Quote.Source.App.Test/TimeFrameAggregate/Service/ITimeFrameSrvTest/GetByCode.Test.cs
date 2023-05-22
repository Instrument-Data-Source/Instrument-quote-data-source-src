

using Instrument.Quote.Source.App.Core.CandleAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Test.Tool;
using Instrument.Quote.Source.App.Test.Tool.SeedData;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.TimeFrameAggregate.Service.ITimeFrameSrvTest;

public class GetByCode_Test : BaseTest
{
  public GetByCode_Test(ITestOutputHelper output) : base(nameof(GetByCode_Test), output)
  {

  }

  [Fact]
  public void WHEN_request_correct_code_THEN_get_correct_response()
  {
    // Array

    // Act
    using var sc = serviceProvider.CreateScope();
    var asserted_dto = sc.ServiceProvider.GetService<ITimeFrameSrv>().GetByCodeAsync("m10").Result;

    // Assert
    Assert.Equal("m10", asserted_dto.Code);
    Assert.Equal(10 * 60, asserted_dto.Seconds);
  }

  [Fact]
  public async Task WHEN_request_wrong_code_THEN_get_not_found_resultAsync()
  {
    // Array

    // Act
    using var sc = serviceProvider.CreateScope();

    // Assert
    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await sc.ServiceProvider.GetService<ITimeFrameSrv>().GetByCodeAsync("x10"));
  }
}