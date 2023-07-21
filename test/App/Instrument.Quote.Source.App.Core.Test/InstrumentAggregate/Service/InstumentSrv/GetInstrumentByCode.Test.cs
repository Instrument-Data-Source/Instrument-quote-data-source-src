using System.Net;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Service.InstrumentSrvTest;
public class GetInstrumentIdByCode_Test
{
  private readonly ITestOutputHelper output;
  private readonly IInstrumentSrv instrumentService;
  private readonly IRepository<ent.Instrument> InstrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private readonly IReadRepository<ent.InstrumentType> InstrumentTypeRep = Substitute.For<IReadRepository<ent.InstrumentType>>();
  public GetInstrumentIdByCode_Test(ITestOutputHelper output)
  {
    this.output = output;
    instrumentService = new InstrumentSrv(InstrumentRep, InstrumentTypeRep);
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public async void WHEN_request_correct_code_THEN_get_dto(bool ToLowerCase)
  {
    // Array
    var instument1 = new ent.Instrument("Inst1", "I1", 2, 3, new ent.InstrumentType(1));
    var instument2 = new ent.Instrument("Inst2", "I2", 1, 2, new ent.InstrumentType(2));
    InstrumentRep.Table.Returns(new[] { instument1, instument2 }.BuildMock());

    var expected_dto = new InstrumentResponseDto() { Id = 0, Name = "Inst1", Code = "I1", TypeId = 1, PriceDecimalLen = 2, VolumeDecimalLen = 3 };
    var usedCode = "I1";
    if (ToLowerCase)
      usedCode = usedCode.ToLower();
      
    // Act
    var asseerted_result = await instrumentService.GetByAsync(usedCode);

    // Assert
    Assert.True(asseerted_result.IsSuccess);
    var assertedDto = Assert.Single(asseerted_result.Value);
    Assert.Equal(expected_dto, assertedDto);
  }

  [Fact]
  public async void WHEN_request_incorrect_code_THEN_get_null()
  {
    // Array
    var instument1 = new ent.Instrument("Inst1", "I1", 2, 3, 1);
    var instument2 = new ent.Instrument("Inst2", "I2", 1, 2, 2);
    InstrumentRep.Table.Returns(new[] { instument1, instument2 }.BuildMock());

    // Act
    var asseerted_result = await instrumentService.GetByAsync("I3");

    // Assert
    Assert.False(asseerted_result.IsSuccess);
  }
}