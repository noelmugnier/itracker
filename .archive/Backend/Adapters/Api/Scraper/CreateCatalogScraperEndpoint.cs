namespace ITracker.Adapters.Api;

public record CreateCatalogScraperRequest(Guid BrandId, Guid CatalogId, string Name, string Url, ParserDto Parser, PaginationDto? Pagination = null, SchedulingDto? Scheduling = null);
public record CreateCatalogScraperResponse(Guid ScraperId);

public class CreateCatalogScraperEndpoint : BaseCommandEndpoint<CreateCatalogScraperRequest, CreateCatalogScraperResponse>
{
	public CreateCatalogScraperEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Post("brands/{brandId}/catalogs/{catalogId}/scrapers");
		AllowAnonymous();
    	Options(x => x.WithTags("Scraper"));
	}

	public override async Task<Result<CreateCatalogScraperResponse>> Handle(CreateCatalogScraperRequest request, CancellationToken token)
	{        
		var result = await Mediator.Execute(new CreateCatalogScraperCommand(request.BrandId, request.CatalogId, request.Name, request.Url, request.Parser, request.Pagination, request.Scheduling), token);
		return result.MapResult(id => new CreateCatalogScraperResponse(id));
	}
}


