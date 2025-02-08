namespace ITracker.Adapters.Api;

public record ListBrandScrapersRequest(Guid BrandId, int PageNumber, int PageSize);
public record ListBrandScrapersResponse(IEnumerable<ScraperDto> Scrapers);

public class ListBrandScrapersEndpoint : BaseQueryEndpoint<ListBrandScrapersRequest, ListBrandScrapersResponse>
{
	public ListBrandScrapersEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/scrapers");
		AllowAnonymous();
    	Options(x => x.WithTags("Scraper")); 
	}

	public override async Task<Result<ListBrandScrapersResponse>> Handle(ListBrandScrapersRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new ListBrandScrapersQuery(request.BrandId, request.PageNumber, request.PageSize), token);
		return result.MapResult(dto => new ListBrandScrapersResponse(dto));
	}
}


