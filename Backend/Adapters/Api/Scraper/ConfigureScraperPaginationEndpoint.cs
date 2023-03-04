namespace ITracker.Adapters.Api;

public record ConfigureScraperPaginationRequest(Guid BrandId, Guid ScraperId, PaginationDto Data);

public class ConfigureScraperPaginationEndpoint : BaseCommandEndpoint<ConfigureScraperPaginationRequest>
{
	public ConfigureScraperPaginationEndpoint(ICommandMediator mediator) : base(mediator)
	{
	}

	public override void Configure()
    {
		Put("brands/{brandId}/scrapers/{scraperId}/pagination");
		AllowAnonymous();
    	Options(x => x.WithTags("Scraper"));
	}

	public override Task<Result> Handle(ConfigureScraperPaginationRequest request, CancellationToken token)
	{
		return Mediator.Execute(new ConfigureScraperPaginationCommand(request.ScraperId, request.Data), token);
	}
}


