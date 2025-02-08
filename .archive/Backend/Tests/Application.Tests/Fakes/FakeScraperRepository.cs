namespace ITracker.Core.Application.Tests;

public class FakeScraperRepository : FakeRepository<ScraperId, Scraper>, IScraperRepository
{
	public Task<Result<IEnumerable<Scraper>>> GetAllForCatalog(CatalogId catalogId, CancellationToken token)
	{
        var scrapers = Set.Values.Where(s => s.CatalogId == catalogId);
        return Task.FromResult(Result.Ok(scrapers));
	}

	public Task<Result<IEnumerable<Scraper>>> GetAllScheduledScrapers(CancellationToken token)
	{
        var scrapers = Set.Values.Where(s => s.SchedulingEnabled);
        return Task.FromResult(Result.Ok(scrapers));
	}
}
