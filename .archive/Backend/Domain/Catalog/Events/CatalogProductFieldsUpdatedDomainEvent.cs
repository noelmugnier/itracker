namespace ITracker.Core.Domain;

public record CatalogProductFieldsUpdatedDomainEvent(CatalogId CatalogId, IEnumerable<PropertyFieldSchema> Fields) : DomainEvent;
