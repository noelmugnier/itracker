namespace ITracker.Adapters.Api;

public record ListCatalogProductsToMapRequest(Guid SourceCatalogId, Guid TargetCatalogId, int PageNumber, int PageSize);
public record ListCatalogProductsToMapResponse(IEnumerable<ProductToMapDto> ProductsToMap);

public class ListCatalogProductsToMapEndpoint : BaseQueryEndpoint<ListCatalogProductsToMapRequest, ListCatalogProductsToMapResponse>
{
	public ListCatalogProductsToMapEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/mappings/{sourceCatalogId}/{targetCatalogId}/pendings");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Catalog"));
	}

	public override async Task<Result<ListCatalogProductsToMapResponse>> Handle(ListCatalogProductsToMapRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new ListCatalogProductsToMapQuery(request.SourceCatalogId, request.TargetCatalogId, request.PageNumber, request.PageSize), token);
		return result.MapResult(dto => new ListCatalogProductsToMapResponse(dto));
	}
}


