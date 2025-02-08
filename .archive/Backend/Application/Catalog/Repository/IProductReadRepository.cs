namespace ITracker.Core.Application;

public interface IProductReadRepository
{
	Task<Result<ProductDto>> FindById(CatalogId catalogId, ProductId productId, CancellationToken token);
	Task<Result<IEnumerable<ProductDto>>> List(CatalogId catalogId, PageNumber pageNumber, PageSize pageSize, CancellationToken token);
}