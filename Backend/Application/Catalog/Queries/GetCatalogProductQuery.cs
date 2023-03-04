namespace ITracker.Core.Application;

public record GetCatalogProductQuery(Guid CatalogId, string ProductId) : IQuery<ProductDto>;

public class GetCatalogProductQueryValidator : AbstractValidator<GetCatalogProductQuery>
{
	public GetCatalogProductQueryValidator()
	{
		RuleFor(x => x.CatalogId).NotEmpty().WithErrorCode(ErrorCode.CatalogIdIsInvalid);
		RuleFor(x => x.ProductId).NotEmpty().WithErrorCode(ErrorCode.ProductIdIsInvalid);
    }
}

internal class GetCatalogProductQueryHandler : IQueryHandler<GetCatalogProductQuery, ProductDto>
{
	private readonly IProductReadRepository _repository;

	public GetCatalogProductQueryHandler(IProductReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<ProductDto>> Handle(GetCatalogProductQuery request, CancellationToken token)
	{
		return _repository.FindById(CatalogId.From(request.CatalogId), ProductId.From(request.ProductId), token);
	}
}
