namespace ITracker.Adapters.Api;

public record GetCatalogProductRequest(Guid BrandId, Guid CatalogId, string ProductId);
public record GetCatalogProductResponse(ProductDto Product);

public class GetCatalogProductEndpoint : BaseQueryEndpoint<GetCatalogProductRequest, GetCatalogProductResponse>
{
	public GetCatalogProductEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/catalogs/{catalogId}/products/{productId}");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Catalog"));
	}

	public override async Task<Result<GetCatalogProductResponse>> Handle(GetCatalogProductRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new GetCatalogProductQuery(request.CatalogId, request.ProductId), token);
		return result.MapResult(dto => new GetCatalogProductResponse(dto));
	}
}


