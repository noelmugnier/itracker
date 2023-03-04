namespace ITracker.Core.Application;

public record ConfigureScraperParserCommand(Guid ScraperId, ParserDto Parser) : ICommand;

public class ConfigureScraperParserCommandValidator : AbstractValidator<ConfigureScraperParserCommand>
{
	public ConfigureScraperParserCommandValidator()
	{
		RuleFor(x => x.ScraperId).NotEmpty().WithErrorCode(ErrorCode.ScraperIdIsRequired);
		RuleFor(x => x.Parser).NotEmpty().WithErrorCode(ErrorCode.ParserIsRequired);
		RuleFor(x => x.Parser).SetValidator(new ParserDtoValidator());
    }
}

internal class ConfigureScraperParserCommandHandler : ICommandHandler<ConfigureScraperParserCommand>
{
	private readonly IScraperFieldsRetriever _scraperFieldsRetriever;
	private readonly IUnitOfWork _uow;

	public ConfigureScraperParserCommandHandler(
		IScraperFieldsRetriever scraperFieldsRetriever,
		IUnitOfWork uow)
	{
		_scraperFieldsRetriever = scraperFieldsRetriever;
		_uow = uow;
	}

	public async Task<Result> Handle(ConfigureScraperParserCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IScraperRepository>();
		var getResult = await repository.Get(ScraperId.From(request.ScraperId), token);
		return await getResult.Match<Task<Result>>(
			async scraper =>
			{				
				var parserBuilder = new ParserBuilder()
					.WithElementSelector(request.Parser.ElementSelector);

				var scraperPropertiesResult = await _scraperFieldsRetriever.RetrieveCatalogScraperFields(scraper.CatalogId, token);
				if(scraperPropertiesResult.IsFailed)
					return scraperPropertiesResult.ToResult();

				foreach (var scraperProperty in scraperPropertiesResult.Value)
				{
					var providedProperty = request.Parser.Properties?.FirstOrDefault(p => p.PropertyName == scraperProperty.PropertyName.Value);
					if(scraperProperty.Required && providedProperty is null)
						return Result.Fail($"Property {scraperProperty.PropertyName.Value} is required");
					
					if(providedProperty is null)
						continue;				

					parserBuilder.AddProperty(scraperProperty.PropertyName.Value, providedProperty.ValueSelector, scraperProperty.ValueType, scraperProperty.Required);
				}

				scraper.ConfigureParser(parserBuilder.Build());
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
