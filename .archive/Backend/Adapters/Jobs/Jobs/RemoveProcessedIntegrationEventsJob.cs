using ITracker.Core.Application;
using Quartz;

namespace ITracker.Adapters.Jobs;

[DisallowConcurrentExecution]
public class RemoveProcessedIntegrationEventsJob : Quartz.IJob
{	
	private readonly IOutboxIntegrationEventsProcessor _outboxIntegrationEventsProcessor;

	public static string CronExpression = "0 * * ? * *";
	public static JobKey JobIdentityKey => new JobKey("RemoveProcessedIntegrationEventsJob", "OutboxIntegrationEvents");
	public static TriggerKey TriggerIdentityKey => new TriggerKey("RemoveProcessedEvents", "OutboxIntegrationEvents");

	public RemoveProcessedIntegrationEventsJob(IOutboxIntegrationEventsProcessor outboxIntegrationEventsProcessor)
	{
		_outboxIntegrationEventsProcessor = outboxIntegrationEventsProcessor;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var result = await _outboxIntegrationEventsProcessor.RemoveProcessedEvents(context.CancellationToken);
		if(result.IsFailed)
			throw new JobExecutionException();
	}
}