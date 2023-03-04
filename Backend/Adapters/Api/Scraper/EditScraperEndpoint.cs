namespace ITracker.Adapters.Api;

public record EditScraperRequest(Guid ScraperId, string Name, string Url);

public class EditScraperEndpoint : BaseCommandEndpoint<EditScraperRequest>
{
	public EditScraperEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Put("brands/{brandId}/scrapers/{scraperId}");
		AllowAnonymous();
    	Options(x => x.WithTags("Scraper"));
	}

	public override Task<Result> Handle(EditScraperRequest request, CancellationToken token)
	{        
		return Mediator.Execute(new EditScraperCommand(request.ScraperId, request.Name, request.Url), token);
	}
}


