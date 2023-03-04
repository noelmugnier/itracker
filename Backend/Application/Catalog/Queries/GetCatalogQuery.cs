namespace ITracker.Core.Application;

public record GetCatalogQuery(Guid CatalogId) : IQuery<CatalogDto>;

public class GetCatalogQueryValidator : AbstractValidator<GetCatalogQuery>
{
	public GetCatalogQueryValidator()
	{
		RuleFor(x => x.CatalogId).NotEmpty().WithErrorCode(ErrorCode.CatalogIdIsInvalid);
    }
}

internal class GetCatalogQueryHandler : IQueryHandler<GetCatalogQuery, CatalogDto>
{
	private readonly ICatalogReadRepository _repository;

	public GetCatalogQueryHandler(ICatalogReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<CatalogDto>> Handle(GetCatalogQuery request, CancellationToken token)
	{
		return _repository.FindById(CatalogId.From(request.CatalogId), token);
	}
}
