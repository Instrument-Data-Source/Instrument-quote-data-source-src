using System.Net;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Test.Tools;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Test.InstrumentAggregate;

public class IInstrumentSrv_Create_Test : BaseDbTest
{

  public IInstrumentSrv_Create_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_create_new_instrument_THEN_instrument_created()
  {
    #region Array
    Logger.LogInformation("Test ARRAY");

    var usingNewInstrumentRequestDto = new NewInstrumentRequestDto()
    {
      Name = "Test instrument 1",
      Code = "TI1",
      TypeId = 1,
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };

    #endregion


    #region Act
    Logger.LogInformation("Test ACT");

    Result<InstrumentResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedInstrumentSrv = sp.GetRequiredService<IInstrumentSrv>();
      assertedResult = await usedInstrumentSrv.CreateAsync(usingNewInstrumentRequestDto);
    }

    #endregion


    #region Assert
    Logger.LogInformation("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Return correct dto", () =>
    {
      Expect("Name is correct", () => Assert.Equal(usingNewInstrumentRequestDto.Name, assertedResult.Value.Name));
      Expect("Code is correct", () => Assert.Equal(usingNewInstrumentRequestDto.Code, assertedResult.Value.Code));
      Expect("PriceDecimalLen is correct", () => Assert.Equal(usingNewInstrumentRequestDto.PriceDecimalLen, assertedResult.Value.PriceDecimalLen));
      Expect("VolumeDecLen is correct", () => Assert.Equal(usingNewInstrumentRequestDto.VolumeDecimalLen, assertedResult.Value.VolumeDecimalLen));
      Expect("TypeId is correct", () => Assert.Equal(usingNewInstrumentRequestDto.TypeId, assertedResult.Value.TypeId));
      Expect("ID > 0", () => Assert.True(assertedResult.Value.Id > 0));
    });

    ExpectGroup("Instrument exist in repository", async () =>
    {
      using (var assert_scope = this.global_sp.CreateScope())
      {
        var sp = assert_scope.ServiceProvider;
        var instrumentRep = sp.GetRequiredService<IReadRepository<ent.Instrument>>();
        var assertedEnt = await instrumentRep.GetByIdAsync(assertedResult.Value.Id);
        Expect("Entity exist", () => Assert.NotNull(assertedEnt));
        ExpectGroup("Entity has correct values", () =>
        {
          Expect("Name is correct", () => Assert.Equal(usingNewInstrumentRequestDto.Name, assertedEnt.Name));
          Expect("Code is correct", () => Assert.Equal(usingNewInstrumentRequestDto.Code, assertedEnt.Code));
          Expect("PriceDecLen is correct", () => Assert.Equal(usingNewInstrumentRequestDto.PriceDecimalLen, assertedEnt.PriceDecimalLen));
          Expect("VolumeDecLen is correct", () => Assert.Equal(usingNewInstrumentRequestDto.VolumeDecimalLen, assertedEnt.VolumeDecimalLen));
          Expect("TypeId is correct", () => Assert.Equal(usingNewInstrumentRequestDto.TypeId, assertedEnt.InstrumentTypeId));
        });
      }
    });
    #endregion
  }

  [Fact]
  public async void WHEN_give_wrong_record_THEN_get_exception()
  {
    #region Array
    this.Logger.LogDebug("Test ARRAY");

    var usingNewInstrumentRequestDto = new NewInstrumentRequestDto()
    {
      Name = "Test instrument 1",
      Code = "TI13.opuo.euoeuoeuoe",
      TypeId = 1,
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };

    #endregion

    using (var act_scope = this.global_sp.CreateScope())
    {
      #region Act
      this.Logger.LogDebug("Test ACT");

      Result<InstrumentResponseDto> assertedResult;

      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<IInstrumentSrv>();



      #endregion


      #region Assert
      this.Logger.LogDebug("Test ASSERT");

      var assertedExpection = await ExpectTaskAsync("Get exception", async () => await Assert.ThrowsAnyAsync<Exception>(async () => await usedTimeFrameSrv.CreateAsync(usingNewInstrumentRequestDto)));
      Logger.LogDebug(assertedExpection.ToString());
      #endregion
    }
  }
  [Fact]
  public async void WHEN_give_wrong_typeId_THEN_get_NotFound()
  {
    #region Array
    this.Logger.LogDebug("Test ARRAY");

    var usingNewInstrumentRequestDto = new NewInstrumentRequestDto()
    {
      Name = "Test instrument 1",
      Code = "TI13",
      TypeId = 99,
      PriceDecimalLen = 2,
      VolumeDecimalLen = 3
    };

    #endregion


    #region Act
    this.Logger.LogDebug("Test ACT");

    Result<InstrumentResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<IInstrumentSrv>();
      assertedResult = await usedTimeFrameSrv.CreateAsync(usingNewInstrumentRequestDto);
    }

    #endregion


    #region Assert
    this.Logger.LogDebug("Test ASSERT");

    Expect("Result is not success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Status is Invalid", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));
    ExpectGroup("Result error is correct", () =>
    {
      Logger.LogDebug(string.Join("\n", assertedResult.Errors));
      Expect("Result contain 1 error", () => Assert.Single(assertedResult.Errors), out var assertedError);
      Expect("Error is TimeFrame", () => Assert.Equal(nameof(ent.InstrumentType), assertedError));
    });

    #endregion
  }
}

