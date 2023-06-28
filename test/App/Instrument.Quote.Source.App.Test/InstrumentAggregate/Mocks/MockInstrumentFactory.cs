using Ardalis.GuardClauses;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;

public class MockInstrumentFactory
{
  private readonly IServiceProvider g_sp;

  public MockInstrumentFactory(IServiceProvider sp)
  {
    this.g_sp = sp;
  }

  public InstrumentResponseDto mockInstrument1 { get; private set; }

  public void Init()
  {
    using var scope = g_sp.CreateScope();
    var sp = scope.ServiceProvider;
    var usingNewInstrumentRequestDto = new NewInstrumentRequestDto()
    {
      Name = "Mock instrument 1",
      Code = "MI1",
      TypeId = 1,
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };
    var usedTimeFrameSrv = sp.GetRequiredService<IInstrumentSrv>();
    var result = Task.Run(() => usedTimeFrameSrv.CreateAsync(usingNewInstrumentRequestDto)).GetAwaiter().GetResult();

    if (!result.IsSuccess)
      throw new ApplicationException("Unexpected fail on init data");

    mockInstrument1 = result.Value;
  }
}