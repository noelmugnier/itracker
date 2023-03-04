namespace ITracker.Core.Application;

public interface IProductHistoryReadRepository
{
	Task<Result<IEnumerable<ProductHistoryDto>>> List(CatalogId catalogId, ProductId productId, PageNumber pageNumber, PageSize pageSize, CancellationToken token);
}