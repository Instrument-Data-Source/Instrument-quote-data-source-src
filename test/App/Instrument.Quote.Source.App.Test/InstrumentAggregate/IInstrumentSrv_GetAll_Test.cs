namespace Instrument.Quote.Source.App.Test.InstrumentAggregate;

using InsonusK.Xunit.ExpectationsTest;
using NSubstitute;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Instrument.Quote.Source.App.Test.Tools;
using Microsoft.Extensions.DependencyInjection;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Interface;
using Instrument.Quote.Source.App.Core.InstrumentAggregate.Dto;
using Instrument.Quote.Source.App.Test.InstrumentAggregate.Mocks;

public class IInstrumentSrv_GetAll_Test : BaseDbTest
{
  public IInstrumentSrv_GetAll_Test(ITestOutputHelper output) : base(output)
  {

  }
  [Fact]
  public async void WHEN_reaquest_all_instrument_THEN_get_all_created_instruments()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    (var expectedDto1, var expectedDto2) = await this.InitInstrumentData();

    #endregion


    #region Act
    Logger.LogDebug("Test ACT");
    IEnumerable<InstrumentResponseDto> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedInstrumentSrv = sp.GetRequiredService<IInstrumentSrv>();
      assertedResult = await usedInstrumentSrv.GetAllAsync();
    }

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Return 2 instrumnent", () => Assert.Equal(2, assertedResult.Count()));
    ExpectGroup("return correct dto", () =>
    {
      Expect("Contain instrumnet 1", () => Assert.Contains(expectedDto1, assertedResult));
      Expect("Contain instrumnet 2", () => Assert.Contains(expectedDto2, assertedResult));
    });

    #endregion
  }
}
