namespace ITracker.Adapters.Api;

public record ListBrandCatalogsRequest(Guid BrandId, int PageNumber, int PageSize);
public record ListBrandCatalogsResponse(IEnumerable<CatalogDto> Catalogs);

public class ListBrandCatalogsEndpoint : BaseQueryEndpoint<ListBrandCatalogsRequest, ListBrandCatalogsResponse>
{
	public ListBrandCatalogsEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/catalogs");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Catalog"));
	}

	public override async Task<Result<ListBrandCatalogsResponse>> Handle(ListBrandCatalogsRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new ListBrandCatalogsQuery(request.BrandId, request.PageNumber, request.PageSize), token);
		return result.MapResult(dto => new ListBrandCatalogsResponse(dto));
	}
}


