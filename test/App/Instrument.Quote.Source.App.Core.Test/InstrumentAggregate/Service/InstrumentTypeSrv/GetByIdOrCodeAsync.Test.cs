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

public class GetByIdOrCodeAsync_Test
{
  private readonly ITestOutputHelper output;
  private readonly IInstrumentTypeSrv instrumentTypeSrv;
  private readonly IReadRepository<InstrumentType> typeRep = Substitute.For<IReadRepository<InstrumentType>>();
  private readonly InstrumentType[] usingTypes;
  public GetByIdOrCodeAsync_Test(ITestOutputHelper output)
  {
    usingTypes = InstrumentType.ToList().Select(e => new InstrumentType(e.Id)).ToArray();
    typeRep.Table.Returns(usingTypes.BuildMock());

    this.output = output;
    instrumentTypeSrv = new InstrumentTypeSrv(output.BuildLoggerFor<InstrumentTypeSrv>(), typeRep);
  }

  [Theory]
  [InlineData("2")]
  [InlineData("Stock")]
  [InlineData("stock")]
  [InlineData("STOCK")]
  public async void WHEN_request_correct_TypeStr_THEN_return_correct_data(string instrumentTypeStr)
  {
    // Array
    var expected_dto = new InstrumentTypeResponseDto()
    {
      Id = 2,
      Name = "Stock"
    };

    // Act
    var asserted_result = await instrumentTypeSrv.GetByIdOrCodeAsync(instrumentTypeStr);

    // Assert
    Assert.True(asserted_result.IsSuccess);
    Assert.Equal(expected_dto, asserted_result.Value);
  }

  [Theory]
  [InlineData("99")]
  [InlineData("Mock")]
  public async void WHEN_request_incorrect_TypeStr_THEN_return_correct_data(string instrumentTypeStr)
  {
    // Array

    // Act
    var asserted_result = await instrumentTypeSrv.GetByIdOrCodeAsync(instrumentTypeStr);

    // Assert
    Assert.False(asserted_result.IsSuccess);
    Assert.Equal(ResultStatus.NotFound, asserted_result.Status);
  }
}