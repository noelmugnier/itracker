namespace ITracker.Core.Application;

public record InitScheduledScraperCommand() : ICommand;

internal class InitScheduledScraperCommandHandler : ICommandHandler<InitScheduledScraperCommand>
{
	private readonly IScraperScheduler _scraperScheduler;
	private readonly IUnitOfWork _uow;

	public InitScheduledScraperCommandHandler(
		IScraperScheduler scraperScheduler,
		IUnitOfWork uow)
	{
		_scraperScheduler = scraperScheduler;
		_uow = uow;
	}
	
	public async Task<Result> Handle(InitScheduledScraperCommand request, CancellationToken token)
	{		
		var repository = _uow.Get<IScraperRepository>();
		var scrapersResult = await repository.GetAllScheduledScrapers(token);
		if(scrapersResult.IsFailed)
			return scrapersResult.ToResult();

		var scrapers = scrapersResult.Value;		
		foreach (var scraper in scrapers.Where(sc => sc.CanBeScheduled))
		{	
			var scheduleResult = await _scraperScheduler.ScheduleScraper(scraper, token);
			if(scheduleResult.IsFailed)
				return scheduleResult;
		}

		return Result.Ok();
	}
}