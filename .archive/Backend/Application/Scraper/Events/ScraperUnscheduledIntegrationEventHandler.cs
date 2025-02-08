namespace ITracker.Core.Application;

public record ScraperUnscheduledIntegrationEvent(Guid ScraperId) : IntegrationEvent;

internal class ScraperUnscheduledIntegrationEventHandler : IntegrationEventHandler<ScraperUnscheduledIntegrationEvent>
{
	private readonly IUnitOfWork _uow;
	private readonly IScraperScheduler _parsingScheduler;

	public ScraperUnscheduledIntegrationEventHandler(
		IUnitOfWork uow,
		IScraperScheduler parsingScheduler)
	{
		_uow = uow;
		_parsingScheduler = parsingScheduler;
	}

	public override async Task<Result> HandleEvent(ScraperUnscheduledIntegrationEvent integrationEvent, CancellationToken token)
	{		
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(integrationEvent.ScraperId), token);
		
		return await getResult.Match<Task<Result>>(
			async scraper => {			
				return !scraper.SchedulingEnabled ? await _parsingScheduler.UnscheduleScraper(scraper, token) : Result.Ok();
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			error => Task.FromResult(Result.Fail(error)));
	}
}
