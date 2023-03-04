namespace ITracker.Core.Domain;

public record ProductTrackedValuesUpdatedDomainEvent(CatalogId CatalogId, ProductId ProductId, Money Price, IEnumerable<PropertyParsed> ModifiedFields) : DomainEvent;
