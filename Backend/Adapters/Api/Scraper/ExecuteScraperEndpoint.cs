namespace ITracker.Adapters.Api;

public record ExecuteScraperRequest(Guid BrandId, Guid ScraperId);

public class ExecuteScraperEndpoint : BaseCommandEndpoint<ExecuteScraperRequest>
{
	public ExecuteScraperEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Post("brands/{brandId}/scrapers/{scraperId}/execute");
		AllowAnonymous();
    	Options(x => x.WithTags("Scraper"));
	}

	public override Task<Result> Handle(ExecuteScraperRequest request, CancellationToken token)
	{        
		return Mediator.Execute(new ExecuteScraperCommand(request.ScraperId), token);
	}
}


