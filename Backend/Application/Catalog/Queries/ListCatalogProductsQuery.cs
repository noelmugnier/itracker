namespace ITracker.Core.Application;

public record ListCatalogProductsQuery(Guid CatalogId, int PageNumber, int PageSize) : IQuery<IEnumerable<ProductDto>>;

public class ListCatalogProductsQueryValidator : AbstractValidator<ListCatalogProductsQuery>
{
	public ListCatalogProductsQueryValidator()
	{
		RuleFor(x => x.CatalogId).NotEmpty().WithErrorCode(ErrorCode.CatalogIdIsInvalid);
		RuleFor(x => x.PageNumber).GreaterThan(0).WithErrorCode(ErrorCode.PageNumberMustBeGreaterThanZero);
		RuleFor(x => x.PageSize).GreaterThan(0).WithErrorCode(ErrorCode.PageSizeMustBeGreaterThanZero);
    }
}

internal class ListCatalogProductsQueryHandler : IQueryHandler<ListCatalogProductsQuery, IEnumerable<ProductDto>>
{
	private readonly IProductReadRepository _repository;

	public ListCatalogProductsQueryHandler(IProductReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<IEnumerable<ProductDto>>> Handle(ListCatalogProductsQuery request, CancellationToken token)
	{
		return _repository.List(CatalogId.From(request.CatalogId), PageNumber.From(request.PageNumber), PageSize.From(request.PageSize), token);
	}
}
