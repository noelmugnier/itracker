namespace ITracker.Adapters.Api;

public record ListCatalogProductsRequest(Guid BrandId, Guid CatalogId, int PageNumber, int PageSize);
public record ListCatalogProductsResponse(IEnumerable<ProductDto> Products);

public class ListCatalogProductsEndpoint : BaseQueryEndpoint<ListCatalogProductsRequest, ListCatalogProductsResponse>
{
	public ListCatalogProductsEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/catalogs/{catalogId}/products");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Catalog"));
	}

	public override async Task<Result<ListCatalogProductsResponse>> Handle(ListCatalogProductsRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new ListCatalogProductsQuery(request.CatalogId, request.PageNumber, request.PageSize), token);
		return result.MapResult(dto => new ListCatalogProductsResponse(dto));
	}
}


