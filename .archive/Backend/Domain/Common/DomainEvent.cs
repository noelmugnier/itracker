namespace ITracker.Core.Domain;

public abstract record DomainEvent
{
	public DomainEventId Id { get; } = DomainEventId.New();
	public DateTimeOffset OccuredOn { get; } = DateTimeOffset.UtcNow;
}
