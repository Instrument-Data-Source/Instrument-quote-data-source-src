using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using NSubstitute;
using Xunit.Abstractions;
using MockQueryable.Moq;

namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Service.InstrumentSrvTest;

public class AddInstrument_Test
{
  private readonly ITestOutputHelper output;
  private readonly IInstrumentSrv instrumentService;
  private readonly IRepository<ent.Instrument> InstrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private readonly IReadRepository<ent.InstrumentType> InstrumentTypeRep = Substitute.For<IReadRepository<ent.InstrumentType>>();
  public AddInstrument_Test(ITestOutputHelper output)
  {
    this.output = output;
    instrumentService = new InstrumentSrv(InstrumentRep, InstrumentTypeRep);
  }

  [Fact]
  public void WHEN_give_valid_type_THEN_get_success_value()
  {
    // Array
    NewInstrumentRequestDto requestDto = new NewInstrumentRequestDto()
    {
      Name = "1",
      Code = "2",
      TypeId = 1
    };

    InstrumentTypeRep.Table.Returns(new List<ent.InstrumentType>() { new InstrumentType((InstrumentType.Enum)1) }.BuildMock());

    // Act
    var asserted_dto = instrumentService.CreateInstrumentAsync(requestDto).Result;

    // Assert
    Assert.NotNull(asserted_dto);
  }

  [Fact]
  public void WHEN_give_valid_data_THEN_add_to_repository_method_will_be_called()
  {
    // Array
    NewInstrumentRequestDto requestDto = new NewInstrumentRequestDto()
    {
      Name = "1",
      Code = "2",
      TypeId = 1
    };

    InstrumentTypeRep.Table.Returns(new List<ent.InstrumentType>() { new InstrumentType((InstrumentType.Enum)1) }.BuildMock());

    // Act
    var asserted_dto = instrumentService.CreateInstrumentAsync(requestDto).Result;

    // Assert
    InstrumentRep.Received().AddAsync(Arg.Any<ent.Instrument>());
  }

  [Fact]
  public async void WHEN_give_invalid_type_THEN_get_invalid_result()
  {
    // Array
    NewInstrumentRequestDto requestDto = new NewInstrumentRequestDto()
    {
      Name = "1",
      Code = "2",
      TypeId = 1
    };


    InstrumentTypeRep.Table.Returns(new List<ent.InstrumentType>().BuildMock());

    // Act

    // Assert
    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await instrumentService.CreateInstrumentAsync(requestDto));
  }

}
