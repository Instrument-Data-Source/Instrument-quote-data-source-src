using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Events;
using Instrument.Quote.Source.App.Core.JoinedChartAggregate.Interface;
using MediatR;
using Quartz;

namespace Instrument.Quote.Source.App.Core.JoinedChartAggregate.Handlers;

public class JoinedChartUpdateRequestedHandler : INotificationHandler<JoinedChartUpdateRequested>
{
  readonly IJoinedChartManager joinedChartManager;
  private readonly ISchedulerFactory schedulerFactory;

  public JoinedChartUpdateRequestedHandler(IJoinedChartManager joinedChartManager, ISchedulerFactory schedulerFactory)
  {
    this.joinedChartManager = joinedChartManager;
    this.schedulerFactory = schedulerFactory;
  }

  public async Task Handle(JoinedChartUpdateRequested notification, CancellationToken cancellationToken)
  {
    var scheduler = await schedulerFactory.GetScheduler();

    var jobKey = $"Id:{notification.JoinedChartId}";
    var job = JobBuilder.Create<CalcJob>()
        .WithIdentity(jobKey, "JoinedChartAggregation")
        .Build();
    job.JobDataMap["JoinedChartId"] = notification.JoinedChartId;

    var trigger = TriggerBuilder.Create()
        .WithIdentity(jobKey, "JoinedChartAggregation")
        .StartNow().ForJob(job)
        .Build();

    await scheduler.ScheduleJob(job, trigger);
  }

  public class CalcJob : IJob
  {
    private readonly IJoinedChartManager joinedChartManager;

    public CalcJob(IJoinedChartManager joinedChartManager)
    {
      this.joinedChartManager = joinedChartManager;
    }
    public async Task Execute(IJobExecutionContext context)
    {
      JobDataMap dataMap = context.JobDetail.JobDataMap;
      var joinedChartId = dataMap.GetIntValue("JoinedChartId");

      await joinedChartManager.UpdateAsync(joinedChartId, context.CancellationToken);
    }
  }
}