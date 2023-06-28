using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Service.InstrumentTypeSrvTest;

public class GetByAsync_Test
{
  private readonly ITestOutputHelper output;
  private readonly IInstrumentTypeSrv instrumentTypeSrv;
  private readonly IReadRepository<InstrumentType> typeRep = Substitute.For<IReadRepository<InstrumentType>>();
  private readonly InstrumentType[] usingTypes;
  public GetByAsync_Test(ITestOutputHelper output)
  {
    usingTypes = InstrumentType.ToList().Select(e => new InstrumentType(e.Id)).ToArray();
    typeRep.Table.Returns(usingTypes.BuildMock());

    this.output = output;
    instrumentTypeSrv = new InstrumentTypeSrv(output.BuildLoggerFor<InstrumentTypeSrv>(), typeRep);
  }

  [Fact]
  public async void WHEN_request_correct_id_THEN_return_correct_data()
  {
    // Array
    var expected_dto = new InstrumentTypeResponseDto()
    {
      Id = 2,
      Name = "Stock"
    };

    // Act
    var asserted_result = await instrumentTypeSrv.GetByAsync(2);

    // Assert
    Assert.True(asserted_result.IsSuccess);
    Assert.Equal(expected_dto, asserted_result.Value);
  }

  [Fact]
  public async void WHEN_request_incorrect_id_THEN_return_correct_data()
  {
    // Array

    // Act
    var asserted_result = await instrumentTypeSrv.GetByAsync(99);

    // Assert
    Assert.False(asserted_result.IsSuccess);
    Assert.Equal(ResultStatus.NotFound, asserted_result.Status);
  }

  [Theory]
  [InlineData("Stock")]
  [InlineData("stock")]
  [InlineData("STOCK")]
  public async void WHEN_request_correct_Name_THEN_return_correct_data(string using_name)
  {
    // Array
    var expected_dto = new InstrumentTypeResponseDto()
    {
      Id = 2,
      Name = "Stock"
    };

    // Act
    var asserted_result = await instrumentTypeSrv.GetByAsync(using_name);

    // Assert
    Assert.True(asserted_result.IsSuccess);
    Assert.Equal(expected_dto, asserted_result.Value);
  }

  [Fact]
  public async void WHEN_request_incorrect_Name_THEN_return_correct_data()
  {
    // Array

    // Act
    var asserted_result = await instrumentTypeSrv.GetByAsync("mock");

    // Assert
    Assert.False(asserted_result.IsSuccess);
    Assert.Equal(ResultStatus.NotFound, asserted_result.Status);
  }
}