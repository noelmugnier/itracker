namespace ITracker.Adapters.Api;

public record GetScraperRequest(Guid BrandId, Guid ScraperId);
public record GetScraperResponse(ScraperDto Scraper);

public class GetScraperEndpoint : BaseQueryEndpoint<GetScraperRequest, GetScraperResponse>
{
	public GetScraperEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/scrapers/{scraperId}");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Scraper"));
	}

	public override async Task<Result<GetScraperResponse>> Handle(GetScraperRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new GetScraperQuery(request.ScraperId), token);
		return result.MapResult(dto => new GetScraperResponse(dto));
	}
}


