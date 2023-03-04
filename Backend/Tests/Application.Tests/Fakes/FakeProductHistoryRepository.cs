namespace ITracker.Core.Application.Tests;

public class FakeProductHistoryRepository : FakeRepository<Guid, ProductHistory>, IProductHistoryRepository
{
	public Task<Result<IEnumerable<ProductHistory>>> GetProductHistory(CatalogId catalogId, ProductId productId, CancellationToken token)
	{
        var result = Set.Values.Where(p => p.CatalogId == catalogId && p.ProductId == productId).ToList();
        return Task.FromResult(Result.Ok<IEnumerable<ProductHistory>>(result));
	}
}
