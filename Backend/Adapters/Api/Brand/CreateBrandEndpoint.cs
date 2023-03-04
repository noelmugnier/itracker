namespace ITracker.Adapters.Api;

public record CreateBrandRequest(string Name, bool IsDefault);
public record CreateBrandResponse(Guid BrandId);

public class CreateBrandEndpoint : BaseCommandEndpoint<CreateBrandRequest, CreateBrandResponse>
{
	public CreateBrandEndpoint(ICommandMediator commandMediator) : base(commandMediator)
	{  
	}

	public override void Configure()
    {
		Post("brands");
		AllowAnonymous();
    	Options(x => x.WithTags("Brand"));
	}

	public override async Task<Result<CreateBrandResponse>> Handle(CreateBrandRequest request, CancellationToken token)
	{   
    Result<Guid> result;
		if(request.IsDefault)
      result = await Mediator.Execute(new CreateDefaultBrandCommand(request.Name), token);
    else 
      result = await Mediator.Execute(new CreateBrandCommand(request.Name), token);

		return result.MapResult(id => new CreateBrandResponse(id));



	}
}
