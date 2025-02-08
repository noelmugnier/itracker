namespace ITracker.Core.Domain;

public interface IProductRepository : IRepository<Product, ProductId>
{
	Task<Result> Insert(Product product, CancellationToken token);
	Task<Result> Update(Product product, CancellationToken token);
}
