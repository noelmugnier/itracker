namespace ITracker.Core.Domain;

public abstract class Entity 
{	
	private List<DomainEvent> _domainEvents = new List<DomainEvent>();
	public IReadOnlyCollection<DomainEvent> DomainEvents => _domainEvents.AsReadOnly();

	public void AddDomainEvent(DomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
}

public abstract class Entity<TId> : Entity, IId<TId>
{
	public Entity(TId id)
	{
		Id = id;
	}

	public TId Id { get; }
}