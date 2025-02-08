namespace ITracker.Core.Application;

public record CreateCatalogScraperCommand(
	Guid BrandId,
	Guid CatalogId,
	string? Name, 
	string Url, 
	ParserDto Parser, 
	PaginationDto? Pagination = null, 
	SchedulingDto? Scheduling = null) : ICommand<Guid>;

public class CreateBrandScraperCommandValidator : AbstractValidator<CreateCatalogScraperCommand>
{
	public CreateBrandScraperCommandValidator()
	{
		RuleFor(x => x.CatalogId).NotEmpty().WithErrorCode(ErrorCode.CatalogIdIsRequired);

		RuleFor(x => x.Url).NotEmpty().WithErrorCode(ErrorCode.ScraperUriIsRequired);
		RuleFor(x => x.Url).Must(IsValidUri).WithErrorCode(ErrorCode.ScraperUriIsInvalid).WithMessage(w => $"{nameof(w.Url)} is invalid");

		RuleFor(x => x.Parser).NotEmpty().WithErrorCode(ErrorCode.ParserIsRequired);
		RuleFor(x => x.Parser).SetValidator(new ParserDtoValidator());

		When(x => x.Pagination is not null, () => {
			RuleFor(x => (PaginationDto)x.Pagination!).SetValidator(new PaginationDtoValidator());
		});

		When(x => x.Scheduling is not null, () => {
			RuleFor(x => (SchedulingDto)x.Scheduling!).SetValidator(new SchedulingDtoValidator());
		});
    }

	public bool IsValidUri(string uri){
		return Uri.TryCreate(uri, new UriCreationOptions(), out _);
	}
}

internal class CreateCatalogScraperCommandHandler : ICommandHandler<CreateCatalogScraperCommand, Guid>
{
	private readonly IScraperFieldsRetriever _scraperFieldsRetriever;
	private readonly IUnitOfWork _uow;

	public CreateCatalogScraperCommandHandler(
		IScraperFieldsRetriever scraperFieldsRetriever,
		IUnitOfWork uow)
	{
		_scraperFieldsRetriever = scraperFieldsRetriever;
		_uow = uow;
	}

	public async Task<Result<Guid>> Handle(CreateCatalogScraperCommand request, CancellationToken token)
	{
		var parserBuilder = new ParserBuilder()
			.WithElementSelector(request.Parser.ElementSelector);

		var catalogId = CatalogId.From(request.CatalogId);
		var scraperPropertiesResult = await _scraperFieldsRetriever.RetrieveCatalogScraperFields(catalogId, token);
		if(scraperPropertiesResult.IsFailed)
			return scraperPropertiesResult.ToResult();

		foreach (var scraperProperty in scraperPropertiesResult.Value)
		{
			var providedProperty = request.Parser.Properties?.FirstOrDefault(p => p.PropertyName == scraperProperty.PropertyName.Value);
			if(scraperProperty.Required && providedProperty is null)
				return Result.Fail<Guid>($"Property {scraperProperty.PropertyName.Value} is required");
			
			if(providedProperty is null)
				continue;				

			parserBuilder.AddProperty(scraperProperty.PropertyName.Value, providedProperty.ValueSelector, scraperProperty.ValueType, scraperProperty.Required);
		}
		
		var scraper = Scraper.Create(BrandId.From(request.BrandId), catalogId, ScraperUri.From(request.Url), parserBuilder.Build(), Name.FromNullable(request.Name));
		
		ConfigurePaginationIfRequired(scraper, request.Pagination);
		ConfigureSchedulingIfRequired(scraper, request.Scheduling);

		var scraperRepository = _uow.Get<IScraperRepository>();
		var insertResult = await scraperRepository.Insert(scraper, token);
		if (insertResult.IsFailed)
			return insertResult;

		var commitResult = await _uow.Commit(token);
		if (commitResult.IsFailed)
			return commitResult;

		return scraper.Id.Value;
	}

	private void ConfigurePaginationIfRequired(Scraper scraper, PaginationDto? pagination)
	{
		if (pagination is null)
			return;

		scraper.ConfigurePagination(
				ParameterName.From(pagination.PageNumberParameterName),
				MaxPages.From(pagination.MaxPages),
				PageSize.FromNullable(pagination.PageSize),
				ParameterName.FromNullable(pagination.PageSizeParameterName));
	}

	private void ConfigureSchedulingIfRequired(Scraper scraper, SchedulingDto? scheduling)
	{
		if (scheduling is null)
			return;

		scraper.ConfigureScheduling(new Cron(scheduling.CronExpression));
	}
}