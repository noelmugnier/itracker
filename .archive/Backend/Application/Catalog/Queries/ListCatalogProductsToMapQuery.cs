namespace ITracker.Core.Application;

public record ListCatalogProductsToMapQuery(Guid SourceCatalogId, Guid TargetCatalogId, int PageNumber, int PageSize) : IQuery<IEnumerable<ProductToMapDto>>;

public class ListCatalogProductsToMapQueryValidator : AbstractValidator<ListCatalogProductsToMapQuery>
{
	public ListCatalogProductsToMapQueryValidator()
	{
		RuleFor(x => x.SourceCatalogId).NotEmpty().WithErrorCode(ErrorCode.SourceCatalogIdIsInvalid);
		RuleFor(x => x.TargetCatalogId).NotEmpty().WithErrorCode(ErrorCode.TargetCatalogIdIsInvalid);
		RuleFor(x => x.PageNumber).GreaterThan(0).WithErrorCode(ErrorCode.PageNumberMustBeGreaterThanZero);
		RuleFor(x => x.PageSize).GreaterThan(0).WithErrorCode(ErrorCode.PageSizeMustBeGreaterThanZero);
    }
}

internal class ListCatalogProductsToMapQueryHandler : IQueryHandler<ListCatalogProductsToMapQuery, IEnumerable<ProductToMapDto>>
{
	private readonly ICatalogProductMappingReadRepository _repository;

	public ListCatalogProductsToMapQueryHandler(ICatalogProductMappingReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<IEnumerable<ProductToMapDto>>> Handle(ListCatalogProductsToMapQuery request, CancellationToken token)
	{
		return _repository.ListPendingProducts(CatalogId.From(request.SourceCatalogId), CatalogId.From(request.TargetCatalogId), PageNumber.From(request.PageNumber), PageSize.From(request.PageSize), token);
	}
}
