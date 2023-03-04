namespace ITracker.Core.Application.Tests;

public class FakeCatalogProductMappingRepository : FakeRepository, ICatalogProductMappingRepository
{
	private List<CatalogProductMapping> _committed = new List<CatalogProductMapping>();
    private List<CatalogProductMapping> _set = new List<CatalogProductMapping>();

	protected IReadOnlyCollection<CatalogProductMapping> Set => _set;
    
    public override void Rollback()
    {
        _set = _committed.ToList();
    }
    
    public override void Commit()
    {
        foreach (var entity in _set)
            entity.ClearDomainEvents();
            
        _committed = _set.ToList();
        _set = _committed.ToList();
    }

    public override IReadOnlyCollection<DomainEvent> GetDomainEvents()
    {
        return _set.SelectMany(x => x.DomainEvents).ToList();
    }

	public Task<Result> Insert(CatalogProductMapping mapping, CancellationToken token)
	{
		_set.Add(mapping);
		return Task.FromResult(Result.Ok());
	}

	public Task<Result> Delete(CatalogId sourceCatalogId, ProductId sourceProductId, CatalogId targetCatalogId, ProductId targetProductId, CancellationToken token)
	{
		var mapping = _set.SingleOrDefault(s => 
			s.SourceCatalogId == sourceCatalogId
			&& s.SourceProductId == sourceProductId
			&& s.TargetCatalogId == targetCatalogId
			&& s.TargetProductId == targetProductId);

		if(mapping == null)
			return Task.FromResult(Result.Ok());

		_set.Remove(mapping);
		return Task.FromResult(Result.Ok());
	}
}
