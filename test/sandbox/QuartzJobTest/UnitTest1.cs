using InsonusK.Xunit.ExpectationsTest;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Xunit.Abstractions;

namespace QuartzJobTest;

public class UnitTest1 : ExpectationsTestBase, IDisposable
{
  IScheduler scheduler;
  public UnitTest1(ITestOutputHelper output, LogLevel logLevel = LogLevel.Debug) : base(output, logLevel)
  {
    StdSchedulerFactory factory = new StdSchedulerFactory();
    scheduler = factory.GetScheduler().Result;
    scheduler.Start().Wait();
  }

  public new void Dispose()
  {
    base.Dispose();
    scheduler.Shutdown().Wait();
    Logger.LogInformation("Scheduler has been shutdowned");
  }

  [Fact]
  public async void Test1()
  {

    //Grab the Scheduler instance from the Factory
    StdSchedulerFactory factory = new StdSchedulerFactory();
    IScheduler scheduler = await factory.GetScheduler();

    // and start it off
    await scheduler.Start();

    // some sleep to show what's happening
    await Task.Delay(TimeSpan.FromSeconds(1));

    // and last shut down the scheduler when you are ready to close your program
    await scheduler.Shutdown();

  }

  [Fact]
  public async void WHEN_run_job_THEN_it_run_correctly()
  {
    #region Array
    Logger.LogDebug("Test ARRAY");
    Quartz.Logging.LogProvider.SetCurrentLogProvider(new ConsoleLogProvider(Output));

    // define the job and tie it to our HelloJob class
    IJobDetail job = JobBuilder.Create<HelloJob>()
      .WithIdentity("job1", "group1")
      .Build();

    ITrigger trigger = TriggerBuilder.Create()
      .WithIdentity("trigger1", "group1")
      .StartNow()
      .Build();
    #endregion


    #region Act
    Logger.LogDebug("Test ACT");

    await scheduler.ScheduleJob(job, trigger);

    await Task.Delay(TimeSpan.FromSeconds(3));

    #endregion


    #region Assert
    Logger.LogDebug("Test ASSERT");

    Expect("Call count of hello job is 1", () => Assert.Equal(1, HelloJob.callCount));

    #endregion
  }

  protected override void Dispose(bool disposing)
  {
    base.Dispose(disposing);
  }

  public class HelloJob : IJob
  {
    public static int callCount = 0;
    public async Task Execute(IJobExecutionContext context)
    {
      callCount++;
      await Console.Out.WriteLineAsync("Greetings from HelloJob!");
    }
  }
}