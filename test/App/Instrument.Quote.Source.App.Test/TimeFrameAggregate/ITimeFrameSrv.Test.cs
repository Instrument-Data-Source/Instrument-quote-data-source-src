using System.Net;
using Ardalis.Result;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Interface;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Test.TimeFrameAggregate;

public class ITimeFrameSrv_GetAll_Test : BaseDbTest<ITimeFrameSrv_GetAll_Test>
{

  public ITimeFrameSrv_GetAll_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public void WHEN_get_request_all_timeframes_THEN_get_list_of_them()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    Result<IEnumerable<TimeFrameResponseDto>> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = usedTimeFrameSrv.GetAllAsync().Result;
    }

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    Expect("Result contain timeframe", () => Assert.Equal(TimeFrame.ToList().Count(), assertedResult.Value.Count()));
    ExpectGroup("Contain timeframes", () =>
    {
      foreach (var item in TimeFrame.ToList().Select(e => new TimeFrame(e.Id)))
      {
        logger.LogInformation($"Check TimeFrame {item.Name}");
        Expect("Has dto with correct id", () => assertedResult.Value.Single(e => e.Id == item.Id), out var assertedDto);
        Expect("Correct name", () => Assert.Equal(item.Name, assertedDto.Code));
        Expect("Correct value in seconds", () => Assert.Equal(item.Seconds, assertedDto.Seconds));
      }
    });

    #endregion
  }
}

public class ITimeFrameSrv_GetById_Test : BaseDbTest<ITimeFrameSrv_GetById_Test>
{

  public ITimeFrameSrv_GetById_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public void WHEN_request_by_exist_id_THEN_get_timeframe()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expectedData = TimeFrame.Enum.D1.ToEntity();

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    Result<TimeFrameResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = usedTimeFrameSrv.GetByIdAsync(expectedData.Id).Result;
    }

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Return correct DTO", () =>
    {
      Expect("ID is correct", () => Assert.Equal(expectedData.Id, assertedResult.Value.Id));
      Expect("Code is correct", () => Assert.Equal(expectedData.Name, assertedResult.Value.Code));
      Expect("Seconds is correct", () => Assert.Equal(expectedData.Seconds, assertedResult.Value.Seconds));
    });

    #endregion
  }

  [Fact]
  public void WHEN_request_incorrect_id_THEN_return_not_found_result()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    Result<TimeFrameResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = usedTimeFrameSrv.GetByIdAsync(9989898).Result;
    }

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));

    #endregion
  }
}


public class ITimeFrameSrv_GetByCode_Test : BaseDbTest<ITimeFrameSrv_GetByCode_Test>
{

  public ITimeFrameSrv_GetByCode_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public void WHEN_request_by_exist_Code_THEN_get_timeframe()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");
    var expectedData = TimeFrame.Enum.D1.ToEntity();

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    Result<TimeFrameResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = usedTimeFrameSrv.GetByCodeAsync(expectedData.Name).Result;
    }

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Return correct DTO", () =>
    {
      Expect("ID is correct", () => Assert.Equal(expectedData.Id, assertedResult.Value.Id));
      Expect("Code is correct", () => Assert.Equal(expectedData.Name, assertedResult.Value.Code));
      Expect("Seconds is correct", () => Assert.Equal(expectedData.Seconds, assertedResult.Value.Seconds));
    });

    #endregion
  }

  [Fact]
  public void WHEN_request_incorrect_code_THEN_return_not_found_result()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    Result<TimeFrameResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<ITimeFrameSrv>();
      assertedResult = usedTimeFrameSrv.GetByCodeAsync("9989898").Result;
    }

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Result is not Success", () => Assert.False(assertedResult.IsSuccess));
    Expect("Result status is NotFound", () => Assert.Equal(ResultStatus.NotFound, assertedResult.Status));

    #endregion
  }
}