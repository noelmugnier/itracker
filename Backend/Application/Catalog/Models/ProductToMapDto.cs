namespace ITracker.Core.Application;

public record ProductToMapDto(Guid SourceCatalogId, string SourceProductId, string Name, Guid TargetCatalogId, string? TargetProductId = null);
