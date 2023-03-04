namespace ITracker.Core.Domain;

public interface ICatalogRepository : IRepository<Catalog, CatalogId>
{
	Task<Result> Insert(Catalog catalog, CancellationToken token);
	Task<Result> Update(Catalog catalog, CancellationToken token);
	Task<Result> Delete(Catalog catalog, CancellationToken token);
}
