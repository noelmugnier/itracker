namespace ITracker.Core.Application;

public record ExecuteScraperCommand(Guid ScraperId) : ICommand;

public class ExecuteScraperCommandValidator : AbstractValidator<ExecuteScraperCommand>
{
	public ExecuteScraperCommandValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsRequired);
    }
}

internal class ExecuteScraperCommandHandler : ICommandHandler<ExecuteScraperCommand>
{
	private readonly IUnitOfWork _uow;
	private readonly ParsingEngine _parsingEngine;

	public ExecuteScraperCommandHandler(IUnitOfWork uow, ParsingEngine parsingEngine)
	{
		_uow = uow;
		_parsingEngine = parsingEngine;
	}

	public async Task<Result> Handle(ExecuteScraperCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(request.ScraperId), token);
		
		return await getResult.Match<Task<Result>>(
			async scraper => {
				var parsingResult = await _parsingEngine.Parse(scraper, token);
				
				var parsingResultRepository = _uow.Get<IParsingResultRepository>();
				await parsingResultRepository.Insert(parsingResult, token);

				return await _uow.Commit(token);	
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			error => Task.FromResult(Result.Fail(error)));		
	}
}