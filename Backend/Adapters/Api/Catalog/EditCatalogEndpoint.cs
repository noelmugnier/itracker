namespace ITracker.Adapters.Api;

public record EditCatalogRequest(Guid CatalogId, string Name);

public class EditCatalogEndpoint : BaseCommandEndpoint<EditCatalogRequest>
{
	public EditCatalogEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Put("brands/{brandId}/catalogs/{catalogId}");
		AllowAnonymous();
    	Options(x => x.WithTags("Catalog"));
	}

	public override Task<Result> Handle(EditCatalogRequest request, CancellationToken token)
	{        
		return Mediator.Execute(new EditCatalogCommand(request.CatalogId, request.Name), token);
	}
}


