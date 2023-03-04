namespace ITracker.Adapters.Api;

public record GetBrandRequest(Guid BrandId);
public record GetBrandResponse(BrandDto Brand);

public class GetBrandEndpoint : BaseQueryEndpoint<GetBrandRequest, GetBrandResponse>
{
	public GetBrandEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands/{brandId}");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Brand"));
	}

	public override async Task<Result<GetBrandResponse>> Handle(GetBrandRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new GetBrandQuery(request.BrandId), token);
		return result.MapResult(dto => new GetBrandResponse(dto));
	}
}


