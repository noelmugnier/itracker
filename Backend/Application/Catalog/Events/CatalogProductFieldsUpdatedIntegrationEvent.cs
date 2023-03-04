namespace ITracker.Core.Application;

public record CatalogProductFieldsUpdatedIntegrationEvent(Guid CatalogId, IEnumerable<PropertyFieldSchema> Fields) : IntegrationEvent;
