namespace ITracker.Core.Application;

public record GetScraperQuery(Guid ScraperId) : IQuery<ScraperDto>;

public class GetScraperQueryValidator : AbstractValidator<GetScraperQuery>
{
	public GetScraperQueryValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsInvalid);
    }
}

internal class GetScraperQueryHandler : IQueryHandler<GetScraperQuery, ScraperDto>
{
	private readonly IScraperReadRepository _repository;

	public GetScraperQueryHandler(IScraperReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<ScraperDto>> Handle(GetScraperQuery request, CancellationToken token)
	{
		return _repository.FindById(ScraperId.From(request.ScraperId), token);
	}
}
