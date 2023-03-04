using ITracker.Core.Application;
using Quartz;

namespace ITracker.Adapters.Jobs;

[DisallowConcurrentExecution]
public class PublishOutboxIntegrationEventsJob : Quartz.IJob
{	
	private readonly IOutboxIntegrationEventsProcessor _outboxIntegrationEventsProcessor;

	public static string CronExpression = "0/15 * * ? * *";
	public static JobKey JobIdentityKey => new JobKey("PublishOutboxIntegrationEventsJob", "OutboxIntegrationEvents");
	public static TriggerKey TriggerIdentityKey => new TriggerKey("PublishPendingEvents", "OutboxIntegrationEvents");

	public PublishOutboxIntegrationEventsJob(IOutboxIntegrationEventsProcessor outboxIntegrationEventsProcessor)
	{
		_outboxIntegrationEventsProcessor = outboxIntegrationEventsProcessor;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var result = await _outboxIntegrationEventsProcessor.PublishPendingEvents(context.CancellationToken);
		if(result.IsFailed)
			throw new JobExecutionException();
	}
}