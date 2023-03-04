namespace ITracker.Core.Application;

public record ListCatalogProductHistoryQuery(Guid CatalogId, string ProductId, int PageNumber, int PageSize) : IQuery<IEnumerable<ProductHistoryDto>>;

public class ListCatalogProductHistoryQueryValidator : AbstractValidator<ListCatalogProductHistoryQuery>
{
	public ListCatalogProductHistoryQueryValidator()
	{
		RuleFor(x => x.CatalogId).NotEmpty().WithErrorCode(ErrorCode.CatalogIdIsInvalid);
		RuleFor(x => x.ProductId).NotEmpty().WithErrorCode(ErrorCode.ProductIdIsInvalid);
		RuleFor(x => x.PageNumber).GreaterThan(0).WithErrorCode(ErrorCode.PageNumberMustBeGreaterThanZero);
		RuleFor(x => x.PageSize).GreaterThan(0).WithErrorCode(ErrorCode.PageSizeMustBeGreaterThanZero);
    }
}

internal class ListCatalogProductHistoryQueryHandler : IQueryHandler<ListCatalogProductHistoryQuery, IEnumerable<ProductHistoryDto>>
{
	private readonly IProductHistoryReadRepository _repository;

	public ListCatalogProductHistoryQueryHandler(IProductHistoryReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<IEnumerable<ProductHistoryDto>>> Handle(ListCatalogProductHistoryQuery request, CancellationToken token)
	{
		return _repository.List(CatalogId.From(request.CatalogId), ProductId.From(request.ProductId), PageNumber.From(request.PageNumber), PageSize.From(request.PageSize), token);
	}
}
