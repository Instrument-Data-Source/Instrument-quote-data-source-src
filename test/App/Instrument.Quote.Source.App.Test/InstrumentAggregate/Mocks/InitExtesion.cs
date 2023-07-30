using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;

namespace Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
public static class BaseDbTestExtension
{
  public static async Task<(InstrumentResponseDto, InstrumentResponseDto)> AddMockInstrumentData(this BaseDbTest baseDbTest)
  {
    var usingNewInstrumentRequestDto1 = new NewInstrumentRequestDto()
    {
      Name = "Test instrument 1",
      Code = "TI1",
      TypeId = 1,
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };
    var usingNewInstrumentRequestDto2 = new NewInstrumentRequestDto()
    {
      Name = "Test instrument 2",
      Code = "TI2",
      TypeId = 2,
      PriceDecimalLen = 3,
      VolumeDecimalLen = 4
    };

    InstrumentResponseDto expectedDto1;
    InstrumentResponseDto expectedDto2;
    using (var act_scope = baseDbTest.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedInstrumentSrv = sp.GetRequiredService<IInstrumentSrv>();
      var assertedResult1 = await usedInstrumentSrv.CreateAsync(usingNewInstrumentRequestDto1);
      var assertedResult2 = await usedInstrumentSrv.CreateAsync(usingNewInstrumentRequestDto2);
      if (!assertedResult1.IsSuccess || !assertedResult2.IsSuccess)
        throw new ApplicationException("Unexpected result");
      expectedDto1 = assertedResult1.Value;
      expectedDto2 = assertedResult2.Value;
      return (expectedDto1, expectedDto2);
    }
  }
}