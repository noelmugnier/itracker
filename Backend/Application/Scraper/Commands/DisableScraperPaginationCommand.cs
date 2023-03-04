namespace ITracker.Core.Application;

public record DisableScraperPaginationCommand(Guid ScraperId) : ICommand;

public class DisableScraperPaginationCommandValidator : AbstractValidator<DisableScraperPaginationCommand>
{
	public DisableScraperPaginationCommandValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsRequired);
    }
}

internal class DisableScraperPaginationCommandHandler : ICommandHandler<DisableScraperPaginationCommand>
{
	private readonly IUnitOfWork _uow;

	public DisableScraperPaginationCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(DisableScraperPaginationCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(request.ScraperId), token);
		return await getResult.Match<Task<Result>>(
			async scraper =>
			{
				scraper.DisablePagination();
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
