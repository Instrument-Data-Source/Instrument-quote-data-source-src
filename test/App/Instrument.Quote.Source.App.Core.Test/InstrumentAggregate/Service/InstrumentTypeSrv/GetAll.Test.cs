namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Service.InstrumentTypeSrvTest;

using System.Net;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
public class GetAll_Test
{
  private readonly ITestOutputHelper output;
  private readonly IInstrumentTypeSrv instrumentTypeSrv;
  private readonly IReadRepository<InstrumentType> typeRep = Substitute.For<IReadRepository<InstrumentType>>();

  public GetAll_Test(ITestOutputHelper output)
  {
    this.output = output;
    instrumentTypeSrv = new InstrumentTypeSrv(output.BuildLoggerFor<InstrumentTypeSrv>(), typeRep);
  }

  [Fact]
  public void WHEN_request_THEN_get_all()
  {
    // Array
    var expected_dto = new[] {
      new InstrumentTypeDto(){Id=1,Name="Currency"},
      new InstrumentTypeDto(){Id=2,Name="Stock"},
      new InstrumentTypeDto(){Id=3,Name="CryptoCurrency"}};
    var usingTypes = InstrumentType.ToList().Select(e => new InstrumentType(e.Id));
    typeRep.Table.Returns(usingTypes.BuildMock());

    // Act
    var asseerted_result = instrumentTypeSrv.GetAllAsync().Result;

    // Assert
    Assert.True(asseerted_result.IsSuccess);
    Assert.Equal(3, asseerted_result.Value.Count());
    foreach (var Dto in asseerted_result.Value)
    {
      expected_dto.Contains(Dto);
    }
  }
}