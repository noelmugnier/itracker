namespace ITracker.Core.Application;

public record CatalogsMappingDto(Guid SourceCatalogId, Guid TargetCatalogId, List<ProductMappingDto> Products);
