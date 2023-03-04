namespace ITracker.Adapters.Api;

public record GetParsingResultRequest(Guid BrandId, Guid ScraperId, Guid ParsingResultId);
public record GetParsingResultResponse(ParsingResultDto ParsingResult);

public class GetParsingResultEndpoint : BaseQueryEndpoint<GetParsingResultRequest, GetParsingResultResponse>
{
	public GetParsingResultEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}/scrapers/{scraperId}/parsings/{parsingResultId}");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Scraper"));
	}

	public override async Task<Result<GetParsingResultResponse>> Handle(GetParsingResultRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new GetParsingResultQuery(request.ParsingResultId), token);
		return result.MapResult(dto => new GetParsingResultResponse(dto));
	}
}


