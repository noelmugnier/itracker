namespace ITracker.Core.Application;

public record ListScraperParsingResultsQuery(Guid ScraperId, int PageNumber, int PageSize) : IQuery<IEnumerable<ParsingResultDto>>;

public class ListScraperParsingResultsQueryValidator : AbstractValidator<ListScraperParsingResultsQuery>
{
	public ListScraperParsingResultsQueryValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsInvalid);
		RuleFor(x => x.PageNumber).GreaterThan(0).WithErrorCode(ErrorCode.PageNumberMustBeGreaterThanZero);
		RuleFor(x => x.PageSize).GreaterThan(0).WithErrorCode(ErrorCode.PageSizeMustBeGreaterThanZero);
    }
}

internal class ListScraperParsingResultsQueryHandler : IQueryHandler<ListScraperParsingResultsQuery, IEnumerable<ParsingResultDto>>
{
	private readonly IParsingResultReadRepository _repository;

	public ListScraperParsingResultsQueryHandler(IParsingResultReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<IEnumerable<ParsingResultDto>>> Handle(ListScraperParsingResultsQuery request, CancellationToken token)
	{
		return _repository.List(ScraperId.From(request.ScraperId), PageNumber.From(request.PageNumber), PageSize.From(request.PageSize), token);
	}
}
