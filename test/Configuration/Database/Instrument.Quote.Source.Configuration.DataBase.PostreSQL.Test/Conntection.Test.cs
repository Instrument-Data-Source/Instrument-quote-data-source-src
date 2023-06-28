namespace Instrument.Quote.Source.Configuration.DataBase.PostreSQL.Test;

using System.Net;
using Instrument.Quote.Source.App.Core.TimeFrameAggregate.Model;
using Instrument.Quote.Source.App.Test.Tools;
using Instrument.Quote.Source.Shared.Kernal.DataBase.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit.Abstractions;
public class Select_Test : BaseDbTest<Select_Test>
{

  public Select_Test(ITestOutputHelper output) :
    base(output, Instrument.Quote.Source.Configuration.DataBase.PostreSQL.Module.Register)
  {

  }
  [Fact]
  public void WHEN_select_to_list_dbContext_THEN_ok()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    List<TimeFrame> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedDbContext = sp.GetRequiredService<SrvDbContext>();
      assertedResult = usedDbContext.TimeFrames.ToList();
    }


    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Response not empty", () => Assert.NotEmpty(assertedResult));

    #endregion
  }

  [Fact]
  public async void WHEN_select_from_Repository_dbContext_THEN_ok()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");

    CancellationToken cancellationToken = default;

    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    IEnumerable<TimeFrame> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedRep = sp.GetRequiredService<IReadRepository<TimeFrame>>();
      assertedResult = await usedRep.Table.ToArrayAsync(cancellationToken);
    }


    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Response not empty", () => Assert.NotEmpty(assertedResult));

    #endregion
  }

  [Fact]
  public async void WHEN_select_with_select_THEN_ok()
  {
    #region Array
    this.logger.LogDebug("Test ARRAY");



    #endregion


    #region Act
    this.logger.LogDebug("Test ACT");
    List<string> assertedResult;
    using (var act_scope = this.global_sp.CreateScope())
    {
      var sp = act_scope.ServiceProvider;
      var usedRep = sp.GetRequiredService<IReadRepository<TimeFrame>>();
      assertedResult = await usedRep.Table.Select(c => c.Name).ToListAsync();
    }


    #endregion


    #region Assert
    this.logger.LogDebug("Test ASSERT");

    Expect("Response not empty", () => Assert.NotEmpty(assertedResult));

    #endregion
  }
}