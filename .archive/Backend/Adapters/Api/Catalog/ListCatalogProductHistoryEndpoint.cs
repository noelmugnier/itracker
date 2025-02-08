namespace ITracker.Adapters.Api;

public record ListCatalogProductHistoryRequest(Guid BrandId, Guid CatalogId, string ProductId, int PageNumber, int PageSize);
public record ListCatalogProductHistoryResponse(IEnumerable<ProductHistoryDto> FieldUpdates);

public class ListCatalogProductHistoryEndpoint : BaseQueryEndpoint<ListCatalogProductHistoryRequest, ListCatalogProductHistoryResponse>
{
	public ListCatalogProductHistoryEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/catalogs/{catalogId}/products/{productId}/history");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Catalog"));
	}

	public override async Task<Result<ListCatalogProductHistoryResponse>> Handle(ListCatalogProductHistoryRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new ListCatalogProductHistoryQuery(request.CatalogId, request.ProductId, request.PageNumber, request.PageSize), token);
		return result.MapResult(dto => new ListCatalogProductHistoryResponse(dto));
	}
}


