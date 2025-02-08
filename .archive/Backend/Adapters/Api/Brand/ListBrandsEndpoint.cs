namespace ITracker.Adapters.Api;

public record ListBrandsRequest(int PageNumber, int PageSize);
public record ListBrandsResponse(IEnumerable<BrandDto> Brands);

public class ListBrandsEndpoint : BaseQueryEndpoint<ListBrandsRequest, ListBrandsResponse>
{
	public ListBrandsEndpoint(IQueryMediator mediator) : base(mediator)
	{ 
	}

	public override void Configure()
    {
		Get("brands");
		AllowAnonymous(); 
    	Options(x => x.WithTags("Brand"));
	}

	public override async Task<Result<ListBrandsResponse>> Handle(ListBrandsRequest request, CancellationToken token)
	{        
		var result = await Mediator.Retrieve(new ListBrandsQuery(request.PageNumber, request.PageSize), token);
		return result.MapResult(dto => new ListBrandsResponse(dto));
	}
}


