namespace ITracker.Core.Domain;

public interface IScraperRepository : IRepository<Scraper, ScraperId>
{
	Task<Result<IEnumerable<Scraper>>> GetAllForCatalog(CatalogId catalogId, CancellationToken token);
	Task<Result<IEnumerable<Scraper>>> GetAllScheduledScrapers(CancellationToken token);
	Task<Result> Insert(Scraper scraper, CancellationToken token);
	Task<Result> Update(Scraper scraper, CancellationToken token);
	Task<Result> Delete(Scraper scraper, CancellationToken token);
}