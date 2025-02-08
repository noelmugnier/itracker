namespace ITracker.Adapters.Api;

public record GetCatalogRequest(Guid BrandId, Guid CatalogId);
public record GetCatalogResponse(CatalogDto Catalog);

public class GetCatalogEndpoint : BaseQueryEndpoint<GetCatalogRequest, GetCatalogResponse>
{
	public GetCatalogEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/catalogs/{catalogId}");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Catalog"));
	}

	public override async Task<Result<GetCatalogResponse>> Handle(GetCatalogRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new GetCatalogQuery(request.CatalogId), token);
		return result.MapResult(dto => new GetCatalogResponse(dto));
	}
}


