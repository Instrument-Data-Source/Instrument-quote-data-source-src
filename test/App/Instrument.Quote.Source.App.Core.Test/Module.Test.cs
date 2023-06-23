namespace Instrument.Quote.Source.App.Core.Test;
using System.Net;
using FluentValidation;
using Instrument.Quote.Source.App.Core.CandleAggregate.Dto;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
public class Module_Test : BaseTest<Module_Test>
{

  public Module_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public void WHEN_call_Register_THEN_validators_register()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    ServiceCollection sc = new ServiceCollection();
    var instrumentRep = Substitute.For<IReadRepository<ent.Instrument>>();
    sc.AddScoped<IReadRepository<ent.Instrument>>(sp => instrumentRep);
    var timeframeRep = Substitute.For<IReadRepository<TimeFrame>>();
    sc.AddScoped<IReadRepository<TimeFrame>>(sp => timeframeRep);
    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");

    var asserted_sc = Module.Register(sc);


    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    using var scope = asserted_sc.BuildServiceProvider().CreateScope();

    var asserted_instance = scope.ServiceProvider.GetService<IValidator<AddCandlesDto>>();

    Expect("Validator register", () =>
    {
      Assert.NotNull(asserted_instance);
    });
    #endregion
  }
}