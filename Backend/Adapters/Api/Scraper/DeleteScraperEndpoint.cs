namespace ITracker.Adapters.Api;

public record DeleteScraperRequest(Guid BrandId, Guid ScraperId);

public class DeleteScraperEndpoint : BaseCommandEndpoint<DeleteScraperRequest>
{
	public DeleteScraperEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
	{
		Delete("brands/{brandId}/scrapers/{scraperId}");
		AllowAnonymous();
    	Options(x => x.WithTags("Scraper"));
	}

	public override Task<Result> Handle(DeleteScraperRequest request, CancellationToken token)
	{        
		return Mediator.Execute(new DeleteScraperCommand(request.ScraperId), token);
	}
}