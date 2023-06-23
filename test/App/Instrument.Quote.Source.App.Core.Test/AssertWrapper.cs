using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Instrument.Quote.Source.App.Core.Test;

public class BaseTest<T> where T : BaseTest<T>
{
  protected readonly ILogger<T> logger;

  public BaseTest(ITestOutputHelper output)
  {
    this.logger = output.BuildLoggerFor<T>();
  }

  protected void Expect(string exception, Action assertAction)
  {
    try
    {
      assertAction.Invoke();
    }
    catch (System.Exception ex)
    {
      this.logger.LogInformation($"{exception} - Failed");
      throw ex;
    }
    this.logger.LogInformation($"{exception} - Checked");
  }
  protected void Expect<TRet>(string exception, Func<TRet> assertFunc, out TRet returnObject)
  {
    try
    {
      returnObject = assertFunc.Invoke();
    }
    catch (System.Exception ex)
    {
      this.logger.LogInformation($"{exception} - Failed");
      throw ex;
    }
    this.logger.LogInformation($"{exception} - Checked");
  }
  protected void ExpectGroup(string exception, Action assertAction)
  {
    this.logger.LogInformation($"{exception} - Checking");
    try
    {
      assertAction.Invoke();
    }
    catch (System.Exception ex)
    {
      this.logger.LogInformation($"{exception} - Failed");
      throw ex;
    }
    this.logger.LogInformation($"{exception} - Checked");
  }
  protected void ExpectGroup<TRet>(string exception, Func<TRet> assertFunc, out TRet returnObject)
  {
    this.logger.LogInformation($"{exception} - Checking");
    try
    {
      returnObject = assertFunc.Invoke();
    }
    catch (System.Exception ex)
    {
      this.logger.LogInformation($"{exception} - Failed");
      throw ex;
    }
    this.logger.LogInformation($"{exception} - Checked");
  }
}