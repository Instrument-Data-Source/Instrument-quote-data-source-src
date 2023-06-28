using System.Net;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Service;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Service.InstrumentSrvTest;
public class Remove_Test
{
  private readonly ITestOutputHelper output;
  private readonly IInstrumentSrv instrumentService;
  private readonly IRepository<ent.Instrument> InstrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private readonly IReadRepository<ent.InstrumentType> InstrumentTypeRep = Substitute.For<IReadRepository<ent.InstrumentType>>();
  public Remove_Test(ITestOutputHelper output)
  {
    this.output = output;
    instrumentService = new InstrumentSrv(InstrumentRep, InstrumentTypeRep);
  }

  [Fact]
  public async void WHEN_remove_exist_instrument_THEN_success_result()
  {
    // Array
    var instument1 = new ent.Instrument("Inst1", "I1", 2, 3, new ent.InstrumentType(1));
    InstrumentRep.Table.Returns(new[] { instument1 }.BuildMock());
    InstrumentRep.RemoveAsync(Arg.Is(instument1), Arg.Any<IDbContextTransaction?>(), Arg.Any<CancellationToken>())
        .Returns(Task.FromResult(true));

    // Act
    var asserted_result = await instrumentService.RemoveAsync(0);

    // Assert
    Assert.True(asserted_result.IsSuccess);
  }

  [Fact]
  public async void WHEN_remove_notexist_instrument_THEN_success_result()
  {
    // Array
    var instument1 = new ent.Instrument("Inst1", "I1", 2, 3, new ent.InstrumentType(1));
    InstrumentRep.Table.Returns(new[] { instument1 }.BuildMock());
    InstrumentRep.RemoveAsync(Arg.Is(instument1), Arg.Any<IDbContextTransaction?>(), Arg.Any<CancellationToken>())
        .Returns(Task.FromResult(true));

    // Act
    var asserted_result = await instrumentService.RemoveAsync(1);

    // Assert
    Assert.False(asserted_result.IsSuccess);
  }
}