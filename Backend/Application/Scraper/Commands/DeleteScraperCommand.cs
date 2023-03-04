namespace ITracker.Core.Application;

public record DeleteScraperCommand(Guid ScraperId) : ICommand;

public class DeleteScraperCommandValidator : AbstractValidator<DeleteScraperCommand>
{
	public DeleteScraperCommandValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsRequired);
	}
}

internal class DeleteScraperCommandHandler : ICommandHandler<DeleteScraperCommand>
{
	private readonly IUnitOfWork _uow;

	public DeleteScraperCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(DeleteScraperCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(request.ScraperId), token);
		
		return await getResult.Match<Task<Result>>(
			async scraper => { 
				var deleteResult = await repository.Delete(scraper, token);
				if (deleteResult.IsFailed)
					return deleteResult;

				var commitResult = await _uow.Commit(token);
				if (commitResult.IsFailed)
					return commitResult;

				return Result.Ok();
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			unexpected => Task.FromResult(Result.Fail(unexpected)));		
	}
}

