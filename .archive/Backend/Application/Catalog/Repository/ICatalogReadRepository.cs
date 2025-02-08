namespace ITracker.Core.Application;

public interface ICatalogReadRepository
{
	Task<Result<CatalogDto>> FindById(CatalogId id, CancellationToken token);
	Task<Result<IEnumerable<CatalogDto>>> List(BrandId brandId, PageNumber pageNumber, PageSize pageSize, CancellationToken token);
}
