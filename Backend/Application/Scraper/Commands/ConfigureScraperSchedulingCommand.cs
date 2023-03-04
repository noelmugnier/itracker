namespace ITracker.Core.Application;

public record ConfigureScraperSchedulingCommand(Guid ScraperId, SchedulingDto Scheduling) : ICommand;

public class ConfigureScraperSchedulingCommandValidator : AbstractValidator<ConfigureScraperSchedulingCommand>
{
	public ConfigureScraperSchedulingCommandValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsRequired);
		RuleFor(x => x.Scheduling).NotEmpty().WithErrorCode(ErrorCode.SchedulingIsRequired);
		RuleFor(x => x.Scheduling).SetValidator(new SchedulingDtoValidator());
    }
}

internal class ConfigureScraperSchedulingCommandHandler : ICommandHandler<ConfigureScraperSchedulingCommand>
{
	private readonly IUnitOfWork _uow;

	public ConfigureScraperSchedulingCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(ConfigureScraperSchedulingCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(request.ScraperId), token);
		return await getResult.Match<Task<Result>>(
			async scraper =>
			{
				scraper.ConfigureScheduling(Cron.From(request.Scheduling.CronExpression));
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