namespace ITracker.Adapters.Api;

public record DeleteBrandRequest(Guid BrandId);

public class DeleteBrandEndpoint : BaseCommandEndpoint<DeleteBrandRequest>
{
	public DeleteBrandEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
	{
		Delete("brands/{brandId}");
		AllowAnonymous();
    	Options(x => x.WithTags("Brand"));
	}

	public override Task<Result> Handle(DeleteBrandRequest request, CancellationToken token)
	{        
		return Mediator.Execute(new DeleteBrandCommand(request.BrandId), token);
	}
}