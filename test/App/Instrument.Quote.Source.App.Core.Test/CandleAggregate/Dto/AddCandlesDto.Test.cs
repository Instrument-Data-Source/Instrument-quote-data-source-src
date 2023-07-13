namespace Instrument.Quote.Source.App.Core.Test.CandleAggregate.Dto;

using System.ComponentModel.DataAnnotations;
using System.Net;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Core.Test.Tools;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using NSubstitute;
using Xunit.Abstractions;
public class AddCandlesDto_Test : BaseTest<AddCandlesDto_Test>
{
  private IRepository<ent.Instrument> instrumentRep = Substitute.For<IRepository<ent.Instrument>>();
  private IRepository<TimeFrame> timeframeRep = Substitute.For<IRepository<TimeFrame>>();
  private ent.Instrument mockInstrument = MockInstrument.Create();
  private TimeFrame usedTf = TimeFrame.Enum.D1.ToEntity();
  private IServiceProvider sp;
  public AddCandlesDto_Test(ITestOutputHelper output) : base(output)
  {
    instrumentRep.Table.Returns(new[] { mockInstrument }.BuildMock());
    timeframeRep.Table.Returns(new[] { usedTf }.BuildMock());
    var sc = new ServiceCollection();
    sc.AddTransient<IReadRepository<TimeFrame>>(sp => timeframeRep);
    sc.AddTransient<IReadRepository<ent.Instrument>>(sp => instrumentRep);
    sp = sc.BuildServiceProvider();
  }

  [Fact]
  public void WHEN_all_id_exist_THEN_dto_valid()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var assertedDto = new NewPeriodDto()
    {
      InstrumentId = mockInstrument.Id,
      TimeFrameId = usedTf.Id
    };

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var validationContext = new ValidationContext(assertedDto, sp, null);
    var validationResults = new List<ValidationResult>();
    var assertedIsValid = Validator.TryValidateObject(assertedDto, validationContext, validationResults, true);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("DTO is valid", () => Assert.True(assertedIsValid));

    #endregion
  }

  [Theory]
  [InlineData(99, null)]
  [InlineData(null, 99)]
  public void WHEN_id_not_exist_THEN_dto_valid(int? instrumenId, int? timeframeId)
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    var assertedDto = new NewPeriodDto()
    {
      InstrumentId = instrumenId ?? mockInstrument.Id,
      TimeFrameId = timeframeId ?? usedTf.Id
    };

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var validationContext = new ValidationContext(assertedDto, sp, null);
    var validationResults = new List<ValidationResult>();
    var assertedIsValid = Validator.TryValidateObject(assertedDto, validationContext, validationResults, true);

    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("DTO is valid", () => Assert.False(assertedIsValid));
    Expect("Validation result contain 1 error", () => Assert.Single(validationResults), out ValidationResult assertedValidationResult);
    Expect("Validation result Member names has one field", () => Assert.Single(assertedValidationResult.MemberNames));
    logger.LogInformation($"Validation result for field {assertedValidationResult.MemberNames} error: {assertedValidationResult.ErrorMessage}");
    #endregion
  }
}