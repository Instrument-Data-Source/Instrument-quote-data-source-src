using InsonusK.Xunit.ExpectationsTest;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Microsoft.Extensions.Hosting;
using NSubstitute;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Instrument.Quote.Source.Configuration.DataBase.PostreSQL;
using Instrument.Quote.Source.Configuration.DataBase;

namespace QuartzJobTest;

public class UsingServiceCollection_Test : IDisposable
{
  private static int magicNumber = 0;
  private bool disposed = false;
  protected readonly string dbSufix;
  protected readonly IServiceProvider global_sp;
  private readonly ITestOutputHelper output;
  IHost host;
  private static void setupConfigBuider(string dbSuffix, IConfigurationBuilder _configurationBuilder)
  {

    _configurationBuilder.AddJsonFile("./appsettings.test.json");
    _configurationBuilder.AddEnvironmentVariables();
    var dict = new Dictionary<string, string>
      {
          {"ConnectionStrings:DbSuffix", dbSuffix},
          {"ASPNETCORE_ENVIRONMENT", "Development"}
      };
    _configurationBuilder.AddInMemoryCollection(dict);

    //Console.WriteLine(_configurationBuilder.Build().GetConnectionString("DefaultConnection"));
    //Console.WriteLine("Db Suffix: " + dbSuffix);
  }
  private string GetDbSufix()
  {
    Interlocked.Increment(ref UsingServiceCollection_Test.magicNumber);
    var timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
    var sufix = $"{this.GetType().Name}_{magicNumber.ToString()}_{timeStamp}";
    return sufix;
  }

  public UsingServiceCollection_Test(ITestOutputHelper output)
  {
    dbSufix = GetDbSufix();
    host = new HostBuilder()
          .ConfigureHostConfiguration(config => setupConfigBuider(dbSufix, config))
          .ConfigureServices((hostContext, services) =>
          {
            // Add required services to the ServiceCollection
            var hostEnvironment = Substitute.For<IHostEnvironment>();
            hostEnvironment.EnvironmentName.Returns("Test");

            services.AddSingleton<ITestOutputHelper>(output);
            services.AddScoped<SubService>();
            services.AddSingleton<IHostEnvironment>(hostEnvironment);
            services.AddLogging(builder =>
                  {
                    builder.AddXunit(output); // Add the xUnit logger
                  });
            services.AddSingleton<ILogger>(sp => output.BuildLogger());

            Module.Register(services);

            services.AddQuartz(q =>
            {
              q.UseMicrosoftDependencyInjectionJobFactory();
              q.UsePersistentStore(options =>
              {
                options.Properties.Add("quartz.jobStore.tablePrefix", "quartz.qrtz_");
                options.PerformSchemaValidation = true;
                options.UseProperties = true;
                options.UseJsonSerializer();
                options.UsePostgres(hostContext.Configuration.GetConnectionStringBuilder().ConnectionString);
              });
            });
            services.AddQuartzHostedService(opt =>
            {
              opt.WaitForJobsToComplete = true;
            });
          })
          .Build();

    global_sp = host.Services;
    host.Start();
    this.output = output;
  }
  public void Dispose()
  {
    try
    {
      using (var scope = global_sp.CreateScope())
      {
        var _sp = scope.ServiceProvider;
        _sp.DeleteDb();

        foreach (var scheduler in _sp.GetRequiredService<ISchedulerFactory>().GetAllSchedulers().Result)
        {
          scheduler.Shutdown().Wait();
        }

      }
    }
    catch (System.Exception ex)
    {
      output.WriteLine("On dispose DeleteDb get excpetion: {0}", ex.ToString());
    }
    try
    {
      host.StopAsync().Wait();
      host.WaitForShutdown();
      host.Dispose();
      output.WriteLine("host.Dispose() - done");
    }
    catch (System.Exception ex)
    {
      output.WriteLine("On dispose host stop get excpetion: {0}", ex.ToString());
    }
    disposed = true;
  }

  //[Fact]
  public async Task WHEN_create_job_THEN_work_correctly()
  {
    #region Array
    //Logger.LogDebug("Test ARRAY");
    using var scope = global_sp.CreateScope();
    var sp = scope.ServiceProvider;

    var schedulerFactory = sp.GetRequiredService<ISchedulerFactory>();

    var scheduler = await schedulerFactory.GetScheduler();

    // define the job and tie it to our HelloJob class
    var job = JobBuilder.Create<HelloJob>()
        .WithIdentity("myJob", "group1")
        .Build();

    // Trigger the job to run now, and then every 40 seconds
    var trigger = TriggerBuilder.Create()
        .WithIdentity("myTrigger", "group1")
        .StartNow()
        .Build();



    // will block until the last running job completes
    //await builder.RunAsync();

    #endregion


    #region Act
    //Logger.LogDebug("Test ACT");

    await scheduler.ScheduleJob(job, trigger);

    for (int i = 0; i < 100; i++)
    {
      if (HelloJob.endCount == 1) break;
      await Task.Delay(TimeSpan.FromMilliseconds(100));
    }
    #endregion


    #region Assert
    //Logger.LogDebug("Test ASSERT");

    //Expect("Call count of hello job is 1", () => Assert.Equal(1, HelloJob.callCount));
    //Expect("End count of hello job is 1", () => Assert.Equal(1, HelloJob.endCount));
    Assert.Equal(1, HelloJob.callCount);
    Assert.Equal(1, HelloJob.endCount);
    #endregion
  }
  //[Fact]
  public async Task WHEN_create_job_THEN_work_correctly2()
  {
    #region Array
    //Logger.LogDebug("Test ARRAY");
    using var scope = global_sp.CreateScope();
    var sp = scope.ServiceProvider;

    var schedulerFactory = sp.GetRequiredService<ISchedulerFactory>();

    var scheduler = await schedulerFactory.GetScheduler();

    // define the job and tie it to our HelloJob class
    var job = JobBuilder.Create<HelloJob>()
        .WithIdentity("myJob", "group1")
        .Build();

    // Trigger the job to run now, and then every 40 seconds
    var trigger = TriggerBuilder.Create()
        .WithIdentity("myTrigger", "group1")
        .StartNow()
        .Build();



    // will block until the last running job completes
    //await builder.RunAsync();

    #endregion


    #region Act
    //Logger.LogDebug("Test ACT");

    await scheduler.ScheduleJob(job, trigger);

    for (int i = 0; i < 100; i++)
    {
      if (HelloJob.endCount == 1) break;
      await Task.Delay(TimeSpan.FromMilliseconds(100));
    }
    #endregion


    #region Assert
    //Logger.LogDebug("Test ASSERT");

    //Expect("Call count of hello job is 1", () => Assert.Equal(1, HelloJob.callCount));
    //Expect("End count of hello job is 1", () =>  Assert.Equal(1, HelloJob.endCount));
    Assert.Equal(1, HelloJob.callCount);
    Assert.Equal(1, HelloJob.endCount);
    #endregion
  }



  public class SubService
  {
    public DateTime WhatTime()
    {
      return DateTime.Now;
    }
  }
  public class HelloJob : IJob
  {
    public static int callCount = 0;
    public static int endCount = 0;
    private readonly SubService clock;
    private readonly ITestOutputHelper output;

    public HelloJob(SubService clock, ITestOutputHelper output)
    {
      this.clock = clock;
      this.output = output;
    }
    public async Task Execute(IJobExecutionContext context)
    {
      callCount++;
      output.WriteLine($"{clock.WhatTime()}: Greetings from HelloJob!");
      Thread.Sleep(1000);
      endCount++;
      output.WriteLine($"{clock.WhatTime()}: HelloJob ends");
    }
  }
}