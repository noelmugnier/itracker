namespace ITracker.Core.Domain;

public record CatalogCreatedDomainEvent(CatalogId CatalogId) : DomainEvent;
