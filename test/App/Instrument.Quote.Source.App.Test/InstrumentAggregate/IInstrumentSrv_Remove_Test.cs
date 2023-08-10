using Ardalis.Result;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;
using Instrument.Quote.Source.App.Test.Tools;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
namespace Instrument.Quote.Source.App.Test.InstrumentAggregate;

public class IInstrumentSrv_Remove_Test : BaseTest
{
  private MockInstrumentFactory mockInstrumentFactory;
  public IInstrumentSrv_Remove_Test(ITestOutputHelper output) : base(output)
  {

  }

  [Fact]
  public async void WHEN_request_remove_THEN_instrument_removed()
  {
    #region Array
    mockInstrumentFactory = new MockInstrumentFactory(hostFixture.Services);
    mockInstrumentFactory.Init();

    this.Logger.LogDebug("Test ARRAY");

    var usingId = mockInstrumentFactory.mockInstrument1.Id;

    #endregion


    #region Act
    this.Logger.LogDebug("Test ACT");

    Result assertedResult;
    using (var act_scope = hostFixture.Services.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedTimeFrameSrv = sp.GetRequiredService<IInstrumentSrv>();
      assertedResult = await usedTimeFrameSrv.RemoveAsync(usingId);
    }

    #endregion


    #region Assert
    this.Logger.LogDebug("Test ASSERT");

    Expect("Result is success", () => Assert.True(assertedResult.IsSuccess));
    ExpectGroup("Instrument does'n exist in repository", async () =>
    {
      using (var assert_scope = hostFixture.Services.CreateScope())
      {
        var sp = assert_scope.ServiceProvider;
        var instrumentRep = sp.GetRequiredService<IReadRepository<ent.Instrument>>();
        var assertedEnt = await instrumentRep.TryGetByIdAsync(usingId);
        Expect("Entity doesn't exist", () => Assert.Null(assertedEnt));
      }
    });

    #endregion
  }
}

