namespace ITracker.Core.Application;

public record EditScraperCommand(
	Guid ScraperId,
	string Name, 
	string Url) : ICommand;

public class EditScraperCommandValidator : AbstractValidator<EditScraperCommand>
{
	public EditScraperCommandValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsRequired);

		RuleFor(x => x.Url).NotEmpty().WithErrorCode(ErrorCode.ScraperUriIsRequired);
		RuleFor(x => x.Url).Must(IsValidUri).WithErrorCode(ErrorCode.ScraperUriIsInvalid).WithMessage(w => $"{nameof(w.Url)} is invalid");
    }

	public bool IsValidUri(string uri){
		return Uri.TryCreate(uri, new UriCreationOptions(), out _);
	}
}

internal class EditScraperCommandHandler : ICommandHandler<EditScraperCommand>
{
	private readonly IUnitOfWork _uow;

	public EditScraperCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(EditScraperCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();

		var scraperResult = await repository.Get(ScraperId.From(request.ScraperId), token);
		return await scraperResult.Match<Task<Result>>(
			async scraper =>
			{
				scraper.Name = Name.From(request.Name);
				scraper.Uri = ScraperUri.From(request.Url);

				var updateResult = await repository.Update(scraper, token);
				if(updateResult.IsFailed)
					return updateResult;

				return await _uow.Commit(token);
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			error => Task.FromResult(Result.Fail(error))
		);
	}
}