using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Events;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Handlers;

public class EventRequestJoinedChartUpdateHandler : INotificationHandler<EventRequestJoinedChartUpdate>
{
  private readonly ISchedulerFactory schedulerFactory;
  private readonly IJoinedChartManager joinedChartManager;
  private readonly ILogger<EventRequestJoinedChartUpdateHandler> logger;

  public EventRequestJoinedChartUpdateHandler(ISchedulerFactory schedulerFactory, IJoinedChartManager joinedChartManager, ILogger<EventRequestJoinedChartUpdateHandler> logger)
  {
    this.schedulerFactory = schedulerFactory;
    this.joinedChartManager = joinedChartManager;
    this.logger = logger;
  }

  public async Task Handle(EventRequestJoinedChartUpdate notification, CancellationToken cancellationToken)
  {
    if (notification.background)
      await RunBackground(notification);
    else
      await joinedChartManager.UpdateAsync(notification.JoinedChartId, cancellationToken);
  }

  private async Task RunBackground(EventRequestJoinedChartUpdate notification)
  {
    var scheduler = await schedulerFactory.GetScheduler();

    var jobDataMap = new JobDataMap();
    jobDataMap.Put("JoinedChartId", notification.JoinedChartId);

    var jobKey = new JobKey($"Id:{notification.JoinedChartId}", "JoinedChartAggregation");
    if (await scheduler.CheckExists(jobKey))
    {
      this.logger.LogInformation("JoinedChartAggregation Job with id {0} already exist", jobKey.Name);
      return;
    }
    var job = JobBuilder.Create<CalcJob>()
        .WithIdentity(jobKey)
        .UsingJobData("JoinedChartId", notification.JoinedChartId)
        .Build();

    var trigger = TriggerBuilder.Create()
        .WithIdentity(jobKey.Name, "JoinedChartAggregation")
        .StartNow().ForJob(job)
        .Build();

    await scheduler.ScheduleJob(job, trigger);
  }

  public class CalcJob : IJob
  {
    private readonly IJoinedChartManager joinedChartManager;
    private readonly ILogger<CalcJob> logger;

    public CalcJob(IJoinedChartManager joinedChartManager, ILogger<CalcJob> logger)
    {
      this.joinedChartManager = joinedChartManager;
      this.logger = logger;
    }
    public async Task Execute(IJobExecutionContext context)
    {
      logger.LogInformation("Begin job: {0}", context.JobDetail.Key);

      logger.LogDebug("Extract JoinedChartId");
      JobDataMap dataMap = context.JobDetail.JobDataMap;
      var joinedChartId = dataMap.GetIntValue("JoinedChartId");

      logger.LogDebug("Start updating joined chart");
      await joinedChartManager.UpdateAsync(joinedChartId, context.CancellationToken);
    }
  }
}