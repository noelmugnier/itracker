namespace ITracker.Core.Application;

public record ListBrandsQuery(int PageNumber, int PageSize) : IQuery<IEnumerable<BrandDto>>;

public class ListBrandsQueryValidator : AbstractValidator<ListBrandsQuery>
{
	public ListBrandsQueryValidator()
	{
		RuleFor(x => x.PageNumber).GreaterThan(0).WithErrorCode(ErrorCode.PageNumberMustBeGreaterThanZero);
		RuleFor(x => x.PageSize).GreaterThan(0).WithErrorCode(ErrorCode.PageSizeMustBeGreaterThanZero);
    }
}

internal class ListBrandsQueryHandler : IQueryHandler<ListBrandsQuery, IEnumerable<BrandDto>>
{
	private readonly IBrandReadRepository _repository;

	public ListBrandsQueryHandler(IBrandReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<IEnumerable<BrandDto>>> Handle(ListBrandsQuery request, CancellationToken token)
	{
		return _repository.List(PageNumber.From(request.PageNumber), PageSize.From(request.PageSize), token);
	}
}
