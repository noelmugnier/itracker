namespace ITracker.Adapters.Api;

public record CreateBrandCatalogRequest(Guid BrandId, string Name, ProductSchemaDto ProductDefinition);
public record CreateBrandCatalogResponse(Guid CatalogId);

public class CreateBrandCatalogEndpoint : BaseCommandEndpoint<CreateBrandCatalogRequest, CreateBrandCatalogResponse>
{
	public CreateBrandCatalogEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Post("brands/{brandId}/catalogs");
		AllowAnonymous();
    	Options(x => x.WithTags("Catalog"));
	}

	public override async Task<Result<CreateBrandCatalogResponse>> Handle(CreateBrandCatalogRequest request, CancellationToken token)
	{        
		var result = await Mediator.Execute(new CreateBrandCatalogCommand(request.BrandId, request.Name, request.ProductDefinition), token);
		return result.MapResult(id => new CreateBrandCatalogResponse(id));
	}
}


