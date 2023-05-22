namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Tool.MapperTest;

using System.Net;
using System.Threading.Tasks;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
using ent = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
public class Dto_ToEntity_Test
{
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  public Dto_ToEntity_Test(ITestOutputHelper output)
  {

  }

  [Fact]
  public async Task WHEN_give_dto_THEN_get_correct_entityAsync()
  {
    // Array
    var using_dto = new CandleDto()
    {
      DateTime = new DateTime(2020, 2, 1).ToUniversalTime(),
      Open = (decimal)1.22,
      High = (decimal)3.5,
      Low = (decimal)0.001,
      Close = (decimal)2.345,
      Volume = (decimal)9.2
    };
    var using_inst_arr = new[] { new ent.Instrument("Instr1", "I1", 3, 2, 1) };
    instrumentRep.Table.Returns(using_inst_arr.BuildMock());

    // Act
    var asserted_entity = await using_dto.ToEntityAsync(0, (int)TimeFrame.Enum.M, instrumentRep);

    // Assert
    Assert.Equal(using_dto.DateTime, asserted_entity.DateTime);

    Assert.Equal(1220, asserted_entity.OpenStore);

    Assert.Equal(3500, asserted_entity.HighStore);

    Assert.Equal(1, asserted_entity.LowStore);

    Assert.Equal(2345, asserted_entity.CloseStore);

    Assert.Equal(920, asserted_entity.VolumeStore);
  }

  [Theory]
  [InlineData(1.3452, 1.22)]
  [InlineData(1.345, 1.233)]
  [InlineData(1.3452, 1.233)]
  public async Task WHEN_decimal_part_longer_than_allowed_THEN_exceptionAsync(decimal close, decimal volume)
  {
    // Array
    var using_dto = new CandleDto()
    {
      DateTime = DateTime.UtcNow,
      Open = (decimal)1.22,
      High = (decimal)1.5,
      Low = (decimal)1.001,
      Close = close,
      Volume = volume
    };
    var using_inst_arr = new[] { new ent.Instrument("Instr1", "I1", 3, 2, 1) };
    instrumentRep.Table.Returns(using_inst_arr.BuildMock());

    // Act

    // Assert
    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await using_dto.ToEntityAsync(0, 1, instrumentRep));
  }
}