namespace ITracker.Adapters.Api;

public record ConfigureCatalogSchemaRequest(Guid CatalogId, ProductSchemaDto ProductDefinition);

public class ConfigureCatalogSchemaEndpoint : BaseCommandEndpoint<ConfigureCatalogSchemaRequest>
{
	public ConfigureCatalogSchemaEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Put("brands/{brandId}/catalogs/{catalogId}/schema");
		AllowAnonymous();
    	Options(x => x.WithTags("Catalog"));
	}

	public override Task<Result> Handle(ConfigureCatalogSchemaRequest request, CancellationToken token)
	{        
		return Mediator.Execute(new ConfigureCatalogSchemaCommand(request.CatalogId, request.ProductDefinition), token);
	}
}


