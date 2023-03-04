namespace ITracker.Core.Application;

public record ScraperScheduledIntegrationEvent(Guid ScraperId, string CronExpression) : IntegrationEvent;

internal class ScraperScheduledIntegrationEventHandler : IntegrationEventHandler<ScraperScheduledIntegrationEvent>
{
	private readonly IUnitOfWork _uow;
	private readonly IScraperScheduler _parsingScheduler;

	public ScraperScheduledIntegrationEventHandler(
		IUnitOfWork uow,
		IScraperScheduler parsingScheduler)
	{
		_uow = uow;
		_parsingScheduler = parsingScheduler;
	}

	public override async Task<Result> HandleEvent(ScraperScheduledIntegrationEvent integrationEvent, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(integrationEvent.ScraperId), token);
		
		return await getResult.Match<Task<Result>>(
			async scraper => {						
				return scraper.SchedulingEnabled ? await _parsingScheduler.ScheduleScraper(scraper, token) : Result.Ok();
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			error => Task.FromResult(Result.Fail(error)));

	}
}
