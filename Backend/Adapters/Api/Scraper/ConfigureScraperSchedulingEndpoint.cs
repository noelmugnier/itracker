namespace ITracker.Adapters.Api;

public record ConfigureScraperSchedulingRequest(Guid BrandId, Guid ScraperId, SchedulingDto Data);

public class ConfigureTrackerSchedulingEndpoint : BaseCommandEndpoint<ConfigureScraperSchedulingRequest>
{
	public ConfigureTrackerSchedulingEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Put("brands/{brandId}/scrapers/{scraperId}/scheduling");
		AllowAnonymous();
    	Options(x => x.WithTags("Scraper"));
	}

	public override Task<Result> Handle(ConfigureScraperSchedulingRequest request, CancellationToken token)
	{
		return Mediator.Execute(new ConfigureScraperSchedulingCommand(request.ScraperId, request.Data), token);
	}
}

