using ITracker.Core.Application;
using ITracker.Core.Domain;
using Quartz;

namespace ITracker.Adapters.Jobs;

[DisallowConcurrentExecution]
public class ExecuteScraperJob : Quartz.IJob
{	
	private readonly ICommandMediator _mediator;

	public static JobKey JobIdentityKey => new JobKey("ExecuteScraperJob", "Scrapers");
	public static TriggerKey GetTriggerIdentityKey(ScraperId scraperId) => new TriggerKey($"{scraperId:N}", "ExecuteScraperJob");

	public Guid ScraperId { private get; set; }

	public ExecuteScraperJob(ICommandMediator mediator)
	{
		_mediator = mediator;
	}

	public async Task Execute(IJobExecutionContext context)
	{
		var result = await _mediator.Execute(new ExecuteScraperCommand(ScraperId), context.CancellationToken);		
	}
}
