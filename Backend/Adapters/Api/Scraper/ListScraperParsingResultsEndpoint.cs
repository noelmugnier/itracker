namespace ITracker.Adapters.Api;

public record ListScraperParsingResultsRequest(Guid BrandId, Guid ScraperId, int PageNumber, int PageSize);
public record ListScraperParsingResultsResponse(IEnumerable<ParsingResultDto> ParsingResults);

public class ListScraperParsingResultsEndpoint : BaseQueryEndpoint<ListScraperParsingResultsRequest, ListScraperParsingResultsResponse>
{
	public ListScraperParsingResultsEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/scrapers/{scraperId}/parsings");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Scraper"));
	}

	public override async Task<Result<ListScraperParsingResultsResponse>> Handle(ListScraperParsingResultsRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new ListScraperParsingResultsQuery(request.ScraperId, request.PageNumber, request.PageSize), token);
		return result.MapResult(dto => new ListScraperParsingResultsResponse(dto));
	}
}


