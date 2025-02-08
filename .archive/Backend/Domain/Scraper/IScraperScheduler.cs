namespace ITracker.Core.Domain;

public interface IScraperScheduler
{
	public Task<Result> ScheduleScraper(Scraper scraper, CancellationToken token);
	public Task<Result> UnscheduleScraper(Scraper scraper, CancellationToken token);
}
