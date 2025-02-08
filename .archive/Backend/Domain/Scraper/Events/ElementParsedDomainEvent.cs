namespace ITracker.Core.Domain;

public record ElementParsedDomainEvent(CatalogId CatalogId, IEnumerable<PropertyParsed> ParsedProperties) : DomainEvent;
