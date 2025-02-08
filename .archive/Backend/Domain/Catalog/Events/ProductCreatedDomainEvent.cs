namespace ITracker.Core.Domain;

public record ProductCreatedDomainEvent(CatalogId CatalogId, ProductId ProductId, Money Price, IEnumerable<PropertyParsed> Fields) : DomainEvent;
