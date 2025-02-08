using ITracker.Core.Domain;

public class CatalogProductMapping : Entity
{
	private CatalogProductMapping() { }

	private CatalogProductMapping(CatalogId sourceCatalogId, ProductId sourceProductId, CatalogId targetCatalogId, ProductId targetProductId)
	{
		SourceCatalogId = sourceCatalogId;
		SourceProductId = sourceProductId;
		TargetCatalogId = targetCatalogId;
		TargetProductId = targetProductId;
	}

	public static CatalogProductMapping Create(CatalogId sourceCatalogId, ProductId sourceProductId, CatalogId targetCatalogId, ProductId targetProductId)
	{
		return new CatalogProductMapping(sourceCatalogId, sourceProductId, targetCatalogId, targetProductId);
	}

	public CatalogId SourceCatalogId { get; }
	public ProductId SourceProductId { get; }

	public CatalogId TargetCatalogId { get; }
	public ProductId TargetProductId { get; }
}
