namespace ITracker.Adapters.Api;

public record DeleteCatalogRequest(Guid CatalogId);

public class DeleteCatalogEndpoint : BaseCommandEndpoint<DeleteCatalogRequest>
{
	public DeleteCatalogEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Delete("brands/{brandId}/catalogs/{catalogId}");
		AllowAnonymous();
    	Options(x => x.WithTags("Catalog"));
	}

	public override Task<Result> Handle(DeleteCatalogRequest request, CancellationToken token)
	{        
		return Mediator.Execute(new DeleteCatalogCommand(request.CatalogId), token);
	}
}


