namespace ITracker.Core.Application;

public record ListBrandScrapersQuery(Guid BrandId, int PageNumber, int PageSize) : IQuery<IEnumerable<ScraperDto>>;

public class ListBrandScrapersQueryValidator : AbstractValidator<ListBrandScrapersQuery>
{
	public ListBrandScrapersQueryValidator()
	{
		RuleFor(x => x.BrandId).NotEmpty().WithErrorCode(ErrorCode.BrandIdIsInvalid);
		RuleFor(x => x.PageNumber).GreaterThan(0).WithErrorCode(ErrorCode.PageNumberMustBeGreaterThanZero);
		RuleFor(x => x.PageSize).GreaterThan(0).WithErrorCode(ErrorCode.PageSizeMustBeGreaterThanZero);
    }
}

internal class ListBrandScrapersQueryHandler : IQueryHandler<ListBrandScrapersQuery, IEnumerable<ScraperDto>>
{
	private readonly IScraperReadRepository _repository;

	public ListBrandScrapersQueryHandler(IScraperReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<IEnumerable<ScraperDto>>> Handle(ListBrandScrapersQuery request, CancellationToken token)
	{
		return _repository.List(BrandId.From(request.BrandId), PageNumber.From(request.PageNumber), PageSize.From(request.PageSize), token);
	}
}
