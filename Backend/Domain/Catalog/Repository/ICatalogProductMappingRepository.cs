namespace ITracker.Core.Domain;

public interface ICatalogProductMappingRepository : IRepository
{
	Task<Result> Insert(CatalogProductMapping mapping, CancellationToken token);
	Task<Result> Delete(CatalogId sourceCatalogId, ProductId sourceProductId, CatalogId targetCatalogId, ProductId targetProductId, CancellationToken token);
}
