using ITracker.Core.Application;
using ITracker.Core.Domain;

namespace ITracker.Adapters.Persistence;

public static class OutboxIntegrationEventMapper
{
	public static IEnumerable<OutboxIntegrationEvent> MapDomainEvents(IEnumerable<DomainEvent> domainEvents)
	{
		return domainEvents.ToList().Select(MapDomainEvent)
			.Where(evt => evt != null)
			.Select(evt => new OutboxIntegrationEvent(evt!))
			.ToList();
	}

	private static IntegrationEvent? MapDomainEvent<T>(T domainEvent) where T: DomainEvent
	{
		return IntegrationEventMapper.MapDomainEvent<T>(domainEvent);
	}
}
