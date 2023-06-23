using System.Net;
using FluentValidation;
using FluentValidation.Results;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.CandleAggregate.Model;
using Instrument.Quote.Source.App.Core.CandleAggregate.Tool;
using Instrument.Quote.Source.App.Core.CandleAggregate.Validator;
using Instrument.Quote.Source.App.Core.Event;
using Instrument.Quote.Source.App.Core.Test;
using Instrument.Quote.Source.App.Core.Test.CandleAggregate.TestData;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
/*
public class AddCandlesDtoValidator_Test : BaseTest<AddCandlesDtoValidator_Test>
{
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private ent.Instrument instrument1;
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private AddCandlesDtoValidator validator;
  private IValidator<NewCandlesDto> candleValidator = Substitute.For<IValidator<NewCandlesDto>>();

  public AddCandlesDtoValidator_Test(ITestOutputHelper output) : base(output)
  {
    base.logger.LogDebug("Mock instrument");
    this.instrument1 = new MockInstrument("Inst1", "I1", 2, 3, new ent.InstrumentType(1)).InitId(10);
    instrumentRep.Table.Returns(new[] { this.instrument1 }.BuildMock());

    base.logger.LogDebug("Mock timeframes");
    var tfs = TimeFrame.ToList().Select(e => new TimeFrame(e.Id)).BuildMock();
    this.timeframeRep.Table.Returns(tfs);

    validator = new AddCandlesDtoValidator(this.instrumentRep, this.timeframeRep, candleValidator);
  }

  [Fact]
  public void WHEN_correct_dto_THEN_return_ok()
  {
    #region Array
    base.logger.LogDebug("Test ARRAY");

    AddCandlesDto usedDto = new AddCandlesDto()
    {
      instrumentId = this.instrument1.Id,
      timeFrameId = (int)TimeFrame.Enum.D1,
      newCandlesDto = new NewCandlesDto()
      {
        From = new DateTime(2020, 1, 1).ToUniversalTime(),
        Untill = new DateTime(2020, 1, 4).ToUniversalTime(),
        Candles = CandleFactory.RandomCandles(3, new DateTime(2020, 1, 1).ToUniversalTime()).Select(e => e.ToDto())
      }
    };
    #endregion


    #region Act
    base.logger.LogDebug("Test ACT");
    var assertedResult = validator.Validate(usedDto);

    #endregion


    #region Assert
    base.logger.LogDebug("Test ASSERT");

    base.Expect("Validation is success", () =>
    {
      Assert.True(assertedResult.IsValid, "Validation result must be Valid");
    });

    base.Expect("Check that newCandlesDto validator has been called", () =>
    {
      candleValidator.Received().Validate(Arg.Is<IValidationContext>(cnxt => cnxt.InstanceToValidate == usedDto.newCandlesDto));
    });
    #endregion
  }

  [Fact]
  public void WHEN_give_wrong_instrument_id_THEN_return_error()
  {
    #region Array
    base.logger.LogDebug("Test ARRAY");

    AddCandlesDto usedDto = new AddCandlesDto()
    {
      instrumentId = this.instrument1.Id + 10,
      timeFrameId = (int)TimeFrame.Enum.D1,
      newCandlesDto = new NewCandlesDto()
      {
        From = new DateTime(2020, 1, 1).ToUniversalTime(),
        Untill = new DateTime(2020, 1, 4).ToUniversalTime(),
        Candles = CandleFactory.RandomCandles(3, new DateTime(2020, 1, 1).ToUniversalTime()).Select(e => e.ToDto())
      }
    };
    #endregion


    #region Act
    base.logger.LogDebug("Test ACT");
    var assertedResult = validator.Validate(usedDto);

    #endregion


    #region Assert
    base.logger.LogDebug("Test ASSERT");

    base.Expect("Validation is failed", () =>
    {
      Assert.False(assertedResult.IsValid);
    });


    base.Expect("Find 1 error of Instrument Id", () =>
    {
      Assert.Equal(1, assertedResult.Errors.Count);
    });

    var assertedError = assertedResult.Errors[0];
    base.Expect("Error is about field Instrument Id", () =>
    {
      Assert.Equal(nameof(usedDto.instrumentId), assertedError.PropertyName);
    });

    base.Expect("Error has correct event id code", () =>
    {
      Assert.Equal(ValidationEvents.IdNotFoundEvent.Id.ToString(), assertedError.ErrorCode);
    });
    #endregion
  }

  [Fact]
  public void WHEN_give_wrong_timeframe_id_THEN_return_error()
  {
    #region Array
    base.logger.LogDebug("Test ARRAY");

    AddCandlesDto usedDto = new AddCandlesDto()
    {
      instrumentId = this.instrument1.Id,
      timeFrameId = 9999,
      newCandlesDto = new NewCandlesDto()
      {
        From = new DateTime(2020, 1, 1).ToUniversalTime(),
        Untill = new DateTime(2020, 1, 4).ToUniversalTime(),
        Candles = CandleFactory.RandomCandles(3, new DateTime(2020, 1, 1).ToUniversalTime()).Select(e => e.ToDto())
      }
    };
    #endregion


    #region Act
    base.logger.LogDebug("Test ACT");
    var assertedResult = validator.Validate(usedDto);

    #endregion


    #region Assert
    base.logger.LogDebug("Test ASSERT");

    base.Expect("Validation is failed", () =>
    {
      Assert.False(assertedResult.IsValid);
    });

    base.Expect("Find 1 error", () =>
     {
       Assert.Equal(1, assertedResult.Errors.Count);
     });

    var assertedError = assertedResult.Errors[0];
    base.Expect("Error is about field TimeFrame Id", () =>
    {
      Assert.Equal(nameof(usedDto.timeFrameId), assertedError.PropertyName);
    });

    base.Expect("Error has correct event id code", () =>
    {
      Assert.Equal(ValidationEvents.IdNotFoundEvent.Id.ToString(), assertedError.ErrorCode);
    });
    #endregion
  }
}
*/