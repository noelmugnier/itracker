namespace ITracker.Core.Domain;

public interface IProductHistoryRepository : IRepository
{
	Task<Result<IEnumerable<ProductHistory>>> GetProductHistory(CatalogId catalogId, ProductId productId, CancellationToken token);
	Task<Result> Insert(ProductHistory product, CancellationToken token);
}