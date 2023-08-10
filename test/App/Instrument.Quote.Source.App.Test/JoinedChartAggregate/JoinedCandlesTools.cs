using Ardalis.Result;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Dto;
using Instrument.Quote.Source.App.Test.Tools;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App.Test.JoinedChartAggregate;
public static class JoinedCandlesTools
{
  public static async Task<Result<GetJoinedCandleResponseDto>> WaitUntillJoinedDataCalculated(this HostFixture hostFixture, GetJoinedChartRequestDto usedRequest)
  {
    Result<GetJoinedCandleResponseDto>? assertedResult = null;
    for (int i = 0; i < 100; i++)
    {
      using (var act_scope = hostFixture.Services.CreateScope())
      {
        var sp = act_scope.ServiceProvider;
        var mediator = sp.GetRequiredService<IMediator>();

        assertedResult = await mediator.Send(usedRequest);
        Assert.True(assertedResult.IsSuccess);
        if (assertedResult.Value.Status == GetJoinedCandleResponseDto.EnumStatus.Ready)
          break;
        Thread.Sleep(100);
      }
    }

    return assertedResult!;
  }
}