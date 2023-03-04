namespace ITracker.Core.Application;

public record ListBrandCatalogsQuery(Guid BrandId, int PageNumber, int PageSize) : IQuery<IEnumerable<CatalogDto>>;

public class ListBrandCatalogsQueryValidator : AbstractValidator<ListBrandCatalogsQuery>
{
	public ListBrandCatalogsQueryValidator()
	{
		RuleFor(x => x.BrandId).NotEmpty().WithErrorCode(ErrorCode.BrandIdIsInvalid);
		RuleFor(x => x.PageNumber).GreaterThan(0).WithErrorCode(ErrorCode.PageNumberMustBeGreaterThanZero);
		RuleFor(x => x.PageSize).GreaterThan(0).WithErrorCode(ErrorCode.PageSizeMustBeGreaterThanZero);
    }
}

internal class ListBrandCatalogsQueryHandler : IQueryHandler<ListBrandCatalogsQuery, IEnumerable<CatalogDto>>
{
	private readonly ICatalogReadRepository _repository;

	public ListBrandCatalogsQueryHandler(ICatalogReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<IEnumerable<CatalogDto>>> Handle(ListBrandCatalogsQuery request, CancellationToken token)
	{
		return _repository.List(BrandId.From(request.BrandId), PageNumber.From(request.PageNumber), PageSize.From(request.PageSize), token);
	}
}
