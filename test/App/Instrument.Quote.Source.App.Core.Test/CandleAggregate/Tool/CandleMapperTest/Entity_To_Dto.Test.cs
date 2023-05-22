namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Tool.MapperTest;

using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
using ent = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
public class Entity_ToDto_Test
{

  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  public Entity_ToDto_Test(ITestOutputHelper output)
  {

  }

  [Fact]
  public void WHEN_give_entity_with_Instrument_property_THEN_get_correct_dto()
  {
    // Array
    var using_entity = new Candle(
      new DateTime(2020, 1, 1).ToUniversalTime(),
      122,
      233,
      1,
      111,
      134,
      (int)TimeFrame.Enum.D1,
      new ent.Instrument("Instr1", "I1", 2, 2, 1));
    //ar using_inst_arr = new[] { new ent.Instrument("Instr1", "I1", 2, 2, 1) };
    //instrumentRep.Table.Returns(using_inst_arr.BuildMock());

    // Act
    var asserted_dto = using_entity.ToDto();

    // Assert
    Assert.Equal(using_entity.DateTime, asserted_dto.DateTime);
    Assert.Equal((decimal)1.22, asserted_dto.Open);
    Assert.Equal((decimal)2.33, asserted_dto.High);
    Assert.Equal((decimal)0.01, asserted_dto.Low);
    Assert.Equal((decimal)1.11, asserted_dto.Close);
    Assert.Equal((decimal)1.34, asserted_dto.Volume);
  }

  [Fact]
  public async void WHEN_give_entity_with_Instrument_Id_THEN_get_exception()
  {
    // Array
    var using_entity = new Candle(
      new DateTime(2020, 1, 1).ToUniversalTime(),
      122,
      233,
      010,
      111,
      134,
      TimeFrame.Enum.D1,
      0);
    //var using_inst_arr = new[] { new ent.Instrument("Instr1", "I1", 2, 2, 1) };
    //instrumentRep.Table.Returns(using_inst_arr.BuildMock());

    // Act

    // Assert
    Assert.Throws<ArgumentException>(() => using_entity.ToDto());
  }
}