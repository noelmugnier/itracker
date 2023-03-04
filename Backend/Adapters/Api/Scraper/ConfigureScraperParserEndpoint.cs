namespace ITracker.Adapters.Api;

public record ConfigureScraperParserRequest(Guid BrandId, Guid ScraperId, ParserDto Data);

public class ConfigureScraperParserEndpoint : BaseCommandEndpoint<ConfigureScraperParserRequest>
{
	public ConfigureScraperParserEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Put("brands/{brandId}/scrapers/{scraperId}/parser");
		AllowAnonymous();
    	Options(x => x.WithTags("Scraper"));
	}

	public override Task<Result> Handle(ConfigureScraperParserRequest request, CancellationToken token)
	{
		return Mediator.Execute(new ConfigureScraperParserCommand(request.ScraperId, request.Data), token);
	}
}