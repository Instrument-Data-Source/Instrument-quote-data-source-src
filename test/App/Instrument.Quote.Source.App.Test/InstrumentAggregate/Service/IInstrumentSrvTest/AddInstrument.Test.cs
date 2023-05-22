using System.Net;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Test.Tool;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Test.InstrumentAggregate.Service.IInstrumentSrvTest;

public class AddInstrument_Test : BaseTest
{

  public AddInstrument_Test(ITestOutputHelper output) : base(nameof(AddInstrument_Test), output)
  {

  }

  [Fact]
  public void WHEN_add_Instrument_THEN_instrument_added()
  {
    // Array
    using var scope = serviceProvider.CreateScope();
    var service = scope.ServiceProvider.GetService<IInstrumentSrv>();
    var newInstrument = new NewInstrumentRequestDto()
    {
      Name = "Inst1",
      Code = "I1",
      TypeId = 1,
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3,
    };
    // Act
    var asserted_dto = service.CreateInstrumentAsync(newInstrument).Result;

    // Assert
    Assert.True(asserted_dto.Id > 0);
    Assert.Equal(asserted_dto.Code, newInstrument.Code);
    Assert.Equal(asserted_dto.Name, newInstrument.Name);
    Assert.Equal(asserted_dto.PriceDecimalLen, newInstrument.PriceDecimalLen);
    Assert.Equal(asserted_dto.VolumeDecimalLen, newInstrument.VolumeDecimalLen);
    Assert.Equal(asserted_dto.Type, "Currency");
  }
}