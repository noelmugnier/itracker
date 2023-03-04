namespace ITracker.Adapters.Api;

public record CreateCatalogsMappingRequest(Guid BrandId, CatalogsMappingDto CatalogsMapping);

public class CreateCatalogsMappingEndpoint : BaseCommandEndpoint<CreateCatalogsMappingRequest>
{
	public CreateCatalogsMappingEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Post("brands/{brandId}/mappings/{sourceCatalogId}/{targetCatalogId}");
		AllowAnonymous();
    	Options(x => x.WithTags("Catalog"));
	}

	public override Task<Result> Handle(CreateCatalogsMappingRequest request, CancellationToken token)
	{
		return Mediator.Execute(new CreateCatalogsMappingCommand(request.BrandId, request.CatalogsMapping), token);
	}
}


