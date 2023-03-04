namespace ITracker.Core.Application;

public interface ICatalogProductMappingReadRepository
{
	Task<Result<IEnumerable<ProductToMapDto>>> ListPendingProducts(CatalogId sourceCatalogId, CatalogId targetCatalogId, PageNumber pageNumber, PageSize pageSize, CancellationToken token);
}
