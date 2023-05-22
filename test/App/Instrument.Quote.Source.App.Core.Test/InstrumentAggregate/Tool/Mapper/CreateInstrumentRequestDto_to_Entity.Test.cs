namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Tool.Mapper;

using System.Net;
using System.Threading.Tasks;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Tool;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
using ent = Instrument.Quote.Source.App.Core.InstrumentAggregate.Model;
public class ToEntity_Test
{
  IReadRepository<ent.InstrumentType> instrumentTypeRep = Substitute.For<IReadRepository<ent.InstrumentType>>();

  public ToEntity_Test(ITestOutputHelper output)
  {

  }

  [Fact]
  public void WHEN_give_entity_with_Type_THEN_convert_equal_entity()
  {
    // Array
    var using_Dto = new NewInstrumentRequestDto()
    {
      Name = "Instrument1",
      Code = "Inst1",
      Type = "Currency",
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };
    var using_IntstrumentTypes = new[] { new ent.InstrumentType(ent.InstrumentType.Enum.Currency) };
    instrumentTypeRep.Table.Returns(using_IntstrumentTypes.BuildMock());

    // Act
    var asserted_ent = using_Dto.ToEntityAsync(instrumentTypeRep).Result;

    // Assert
    Assert.Equal(using_Dto.Name, asserted_ent.Name);
    Assert.Equal(using_Dto.Code, asserted_ent.Code);
    Assert.Equal(using_IntstrumentTypes[0], asserted_ent.InstrumentType);
    Assert.Equal(using_IntstrumentTypes[0].Id, asserted_ent.InstrumentTypeId);
    Assert.Equal(using_Dto.PriceDecimalLen, asserted_ent.PriceDecimalLen);
    Assert.Equal(using_Dto.VolumeDecimalLen, asserted_ent.VolumeDecimalLen);
  }

  [Fact]
  public void WHEN_give_entity_with_Type_Id_THEN_convert_equal_entity()
  {
    // Array
    var using_Dto = new NewInstrumentRequestDto()
    {
      Name = "Instrument1",
      Code = "Inst1",
      TypeId = (int)ent.InstrumentType.Enum.Currency,
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };
    var using_IntstrumentTypes = new[] { new ent.InstrumentType(ent.InstrumentType.Enum.Currency) };
    instrumentTypeRep.Table.Returns(using_IntstrumentTypes.BuildMock());

    // Act
    var asserted_ent = using_Dto.ToEntityAsync(instrumentTypeRep).Result;

    // Assert
    Assert.Equal(using_Dto.Name, asserted_ent.Name);
    Assert.Equal(using_Dto.Code, asserted_ent.Code);
    Assert.Equal(using_IntstrumentTypes[0], asserted_ent.InstrumentType);
    Assert.Equal(using_IntstrumentTypes[0].Id, asserted_ent.InstrumentTypeId);
    Assert.Equal(using_Dto.PriceDecimalLen, asserted_ent.PriceDecimalLen);
    Assert.Equal(using_Dto.VolumeDecimalLen, asserted_ent.VolumeDecimalLen);
  }
  [Fact]
  public void WHEN_give_Type_and_TypeId_THEN_convert_equal_entity()
  {
    // Array
    var using_Dto = new NewInstrumentRequestDto()
    {
      Name = "Instrument1",
      Code = "Inst1",
      TypeId = (int)ent.InstrumentType.Enum.Stock,
      Type = "Stock",
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };
    var using_IntstrumentTypes = new[] { new ent.InstrumentType(ent.InstrumentType.Enum.Currency), new ent.InstrumentType(ent.InstrumentType.Enum.Stock) };
    instrumentTypeRep.Table.Returns(using_IntstrumentTypes.BuildMock());

    // Act
    var asserted_ent = using_Dto.ToEntityAsync(instrumentTypeRep).Result;

    // Assert
    Assert.Equal(using_Dto.Name, asserted_ent.Name);
    Assert.Equal(using_Dto.Code, asserted_ent.Code);
    Assert.Equal(using_IntstrumentTypes[1], asserted_ent.InstrumentType);
    Assert.Equal(using_IntstrumentTypes[1].Id, asserted_ent.InstrumentTypeId);
    Assert.Equal(using_Dto.PriceDecimalLen, asserted_ent.PriceDecimalLen);
    Assert.Equal(using_Dto.VolumeDecimalLen, asserted_ent.VolumeDecimalLen);
  }
  [Fact]
  public async Task WHEN_give_wrong_Type_THEN_get_exceptionAsync()
  {
    // Array
    var using_Dto = new NewInstrumentRequestDto()
    {
      Name = "Instrument1",
      Code = "Inst1",
      Type = "NOTCurrency",
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };
    var using_IntstrumentTypes = new[] { new ent.InstrumentType(ent.InstrumentType.Enum.Currency) };
    instrumentTypeRep.Table.Returns(using_IntstrumentTypes.BuildMock());

    // Act

    // Assert
    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await using_Dto.ToEntityAsync(instrumentTypeRep));
  }

  [Fact]
  public async Task WHEN_give_wrong_Type_Id_THEN_get_exceptionAsync()
  {
    // Array
    var using_Dto = new NewInstrumentRequestDto()
    {
      Name = "Instrument1",
      Code = "Inst1",
      TypeId = 9999999,
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };
    var using_IntstrumentTypes = new[] { new ent.InstrumentType(ent.InstrumentType.Enum.Currency) };
    instrumentTypeRep.Table.Returns(using_IntstrumentTypes.BuildMock());

    // Act

    // Assert
    await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await using_Dto.ToEntityAsync(instrumentTypeRep));
  }

  [Fact]
  public async Task WHEN_Type_NE_TypeId_THEN_get_exceptionAsync()
  {
    // Array
    var using_Dto = new NewInstrumentRequestDto()
    {
      Name = "Instrument1",
      Code = "Inst1",
      TypeId = (int)ent.InstrumentType.Enum.Currency,
      Type = "Stock",
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };
    var using_IntstrumentTypes = new[] { new ent.InstrumentType(ent.InstrumentType.Enum.Currency), new ent.InstrumentType(ent.InstrumentType.Enum.Stock) };
    instrumentTypeRep.Table.Returns(using_IntstrumentTypes.BuildMock());

    // Act

    // Assert
    await Assert.ThrowsAsync<ArgumentException>(async () => await using_Dto.ToEntityAsync(instrumentTypeRep));
  }
}