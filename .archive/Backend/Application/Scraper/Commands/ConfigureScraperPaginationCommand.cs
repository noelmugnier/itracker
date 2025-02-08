namespace ITracker.Core.Application;

public record ConfigureScraperPaginationCommand(Guid ScraperId, PaginationDto Pagination) : ICommand;

public class ConfigureScraperPaginationCommandValidator : AbstractValidator<ConfigureScraperPaginationCommand>
{
	public ConfigureScraperPaginationCommandValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsRequired);
		RuleFor(x => x.Pagination).NotEmpty().WithErrorCode(ErrorCode.PaginationIsRequired);
		RuleFor(x => x.Pagination).SetValidator(new PaginationDtoValidator());
    }
}

internal class ConfigureScraperPaginationCommandHandler : ICommandHandler<ConfigureScraperPaginationCommand>
{
	private readonly IUnitOfWork _uow;

	public ConfigureScraperPaginationCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(ConfigureScraperPaginationCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(request.ScraperId), token);
		return await getResult.Match<Task<Result>>(
			async scraper =>
			{
				scraper.ConfigurePagination(
					ParameterName.From(request.Pagination.PageNumberParameterName),
					MaxPages.From(request.Pagination.MaxPages),
					PageSize.FromNullable(request.Pagination.PageSize),
					ParameterName.FromNullable(request.Pagination.PageSizeParameterName));

				var updateResult = await repository.Update(scraper, token);
				if (updateResult.IsFailed)
					return updateResult;

				return await _uow.Commit(token);
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			unexpected => Task.FromResult(Result.Fail(unexpected))
		);
	}
}
