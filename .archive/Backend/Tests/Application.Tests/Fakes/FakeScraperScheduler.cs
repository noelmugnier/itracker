namespace ITracker.Core.Application.Tests;

public class FakeScraperScheduler : IScraperScheduler
{
	public List<Scraper> ScheduledScrapers = new();

	public Task<Result> ScheduleScraper(Scraper scraper, CancellationToken token)
	{
		var existingScraper = ScheduledScrapers.FirstOrDefault(s => s.Id == scraper.Id);
		if(existingScraper != null)
			ScheduledScrapers.Remove(existingScraper);

		ScheduledScrapers.Add(scraper);
		return Task.FromResult(Result.Ok());
	}

	public Task<Result> UnscheduleScraper(Scraper scraper, CancellationToken token)
	{
		var existingScraper = ScheduledScrapers.FirstOrDefault(s => s.Id == scraper.Id);
		if(existingScraper != null)
			ScheduledScrapers.Remove(existingScraper);

		return Task.FromResult(Result.Ok());
	}
}