namespace ITracker.Core.Application;

public record DisableScraperSchedulingCommand(Guid ScraperId) : ICommand;

public class DisableScraperSchedulingCommandValidator : AbstractValidator<DisableScraperSchedulingCommand>
{
	public DisableScraperSchedulingCommandValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsRequired);
    }
}

internal class DisableScraperSchedulingCommandHandler : ICommandHandler<DisableScraperSchedulingCommand>
{
	private readonly IUnitOfWork _uow;

	public DisableScraperSchedulingCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(DisableScraperSchedulingCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(request.ScraperId), token);
		return await getResult.Match(
			async brand =>
			{
				brand.DisableScheduling();
				var updateResult = await repository.Update(brand, token);
				if (updateResult.IsFailed)
					return updateResult;
				
				return await _uow.Commit(token);
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			unexpected => Task.FromResult(Result.Fail(unexpected))
		);
	}
}