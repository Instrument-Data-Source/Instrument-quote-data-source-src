namespace Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Tool.Mapper;

using System.Threading.Tasks;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Tool;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
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
  /*
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
      var asserted_ent = await using_Dto.ToEntityAsync(instrumentTypeRep);

      // Assert
      Assert.Equal(using_Dto.Name, asserted_ent.Name);
      Assert.Equal(using_Dto.Code, asserted_ent.Code);
      Assert.Equal(using_IntstrumentTypes[0].Id, asserted_ent.InstrumentTypeId);
      Assert.Equal(using_Dto.PriceDecimalLen, asserted_ent.PriceDecimalLen);
      Assert.Equal(using_Dto.VolumeDecimalLen, asserted_ent.VolumeDecimalLen);
    }*/

  [Fact]
  public async void WHEN_give_entity_with_Type_Id_THEN_convert_equal_entity()
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
    var asserted_ent_res = await using_Dto.ToEntityAsync(instrumentTypeRep);

    // Assert
    Assert.True(asserted_ent_res.IsSuccess);
    var assertedValue = asserted_ent_res.Value;
    Assert.Equal(using_Dto.Name, assertedValue.Name);
    Assert.Equal(using_Dto.Code, assertedValue.Code);
    Assert.Equal(using_IntstrumentTypes[0].Id, assertedValue.InstrumentTypeId);
    Assert.Equal(using_Dto.PriceDecimalLen, assertedValue.PriceDecimalLen);
    Assert.Equal(using_Dto.VolumeDecimalLen, assertedValue.VolumeDecimalLen);
  }
  /*
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
    var asserted_ent = await using_Dto.ToEntityAsync(instrumentTypeRep);

    // Assert
    Assert.Equal(using_Dto.Name, asserted_ent.Name);
    Assert.Equal(using_Dto.Code, asserted_ent.Code);
    Assert.Equal(using_IntstrumentTypes[1].Id, asserted_ent.InstrumentTypeId);
    Assert.Equal(using_Dto.PriceDecimalLen, asserted_ent.PriceDecimalLen);
    Assert.Equal(using_Dto.VolumeDecimalLen, asserted_ent.VolumeDecimalLen);
  }
  */
  /*
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
    await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await using_Dto.ToEntityAsync(instrumentTypeRep));
  }
*/
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
    var assertedResult = await using_Dto.ToEntityAsync(instrumentTypeRep);
    // Assert
    Assert.False(assertedResult.IsSuccess);
    Assert.Equal(ResultStatus.NotFound, assertedResult.Status);
    var assertedError = Assert.Single(assertedResult.Errors);
    Assert.Equal(nameof(ent.InstrumentType), assertedError);
  }
  /*
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
      await Assert.ThrowsAsync<FluentValidation.ValidationException>(async () => await using_Dto.ToEntityAsync(instrumentTypeRep));
    }*/
}