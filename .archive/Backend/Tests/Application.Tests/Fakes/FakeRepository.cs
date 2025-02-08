namespace ITracker.Core.Application.Tests;

public abstract class FakeRepository : IRepository
{
    public abstract IReadOnlyCollection<DomainEvent> GetDomainEvents();
    public abstract void Rollback();
    public abstract void Commit();
}

public abstract class FakeRepository<TId, T> : FakeRepository
    where T : Entity<TId>
    where TId : notnull
{
	private Dictionary<TId, T> _committed = new Dictionary<TId, T>();
    private Dictionary<TId, T> _set = new Dictionary<TId, T>();

	protected IReadOnlyDictionary<TId, T> Set => _set;

    public Task<OneOf<T, NotFoundError, UnexpectedError>> Get(TId id, CancellationToken token)
    {
        return Task.FromResult<OneOf<T, NotFoundError, UnexpectedError>>(Set.TryGetValue(id, out var entity) ? entity : NotFoundError.From<TId>(id));
    }

    public Task<Result> Insert(T entity, CancellationToken token)
    {
        _set.Add(entity.Id, entity);
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> Update(T entity, CancellationToken token)
    {
        _set.Remove(entity.Id, out var existingEntity);
        _set.Add(entity.Id, entity);
        
        return Task.FromResult(Result.Ok());
    }

    public Task<Result> Delete(T entity, CancellationToken token)
    {
        _set.Remove(entity.Id, out var existingEntity);
        return Task.FromResult(Result.Ok());
    }
    
    public override void Rollback()
    {
        _set = _committed.ToDictionary(x => x.Key, x => x.Value);
    }
    
    public override void Commit()
    {
        foreach (var entity in _set.Values)
            entity.ClearDomainEvents();
            
        _committed = _set.ToDictionary(x => x.Key, x => x.Value);
        _set = _committed.ToDictionary(x => x.Key, x => x.Value);
    }

    public override IReadOnlyCollection<DomainEvent> GetDomainEvents()
    {
        return _set.Values.SelectMany(x => x.DomainEvents).ToList();
    }
}
