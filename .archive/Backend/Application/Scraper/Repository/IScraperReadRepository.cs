namespace ITracker.Core.Application;

public interface IScraperReadRepository
{
	Task<Result<ScraperDto>> FindById(ScraperId scraperId, CancellationToken token);
	Task<Result<IEnumerable<ScraperDto>>> List(BrandId brandId, PageNumber pageNumber, PageSize pageSize, CancellationToken token);
}
