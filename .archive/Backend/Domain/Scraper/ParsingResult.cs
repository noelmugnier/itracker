namespace ITracker.Core.Domain;

public class ParsingResult : Entity<ParsingResultId>
{
	private readonly List<ParsedElement> _elements = new List<ParsedElement>();
	private readonly List<ParsingError> _errors = new List<ParsingError>();
	
	private ParsingResult()
		: base(ParsingResultId.New())
	{ }

	private ParsingResult(ScraperId scraperId, ParsingStatus status, DateRange processingTime)
		: base(ParsingResultId.New())
	{
		CreatedOn = DateTimeOffset.UtcNow;
		Status = status;
		StartedOn = processingTime.Start;
		EndedOn = processingTime.End;
		ScraperId = scraperId;

		switch(status)
		{
			case ParsingStatus.Completed:
				AddDomainEvent(new ParsingCompletedDomainEvent(Id));
				break;
			case ParsingStatus.Failed:
				AddDomainEvent(new ParsingFailedDomainEvent(Id));
				break;
			case ParsingStatus.CompletedWithErrors:
				AddDomainEvent(new ParsingCompletedWithErrorsDomainEvent(Id));
				break;
		}
	}

	public ParsingStatus Status { get; private set; }
	public DateTimeOffset CreatedOn { get; }
	public DateTimeOffset StartedOn { get; }
	public DateTimeOffset EndedOn { get; }
	public ScraperId ScraperId { get; }
	public bool HasErrors => Errors.Any();
	public IReadOnlyCollection<ParsedElement> Elements => _elements.AsReadOnly();
	public IReadOnlyCollection<ParsingError> Errors => _errors.AsReadOnly();

	private void AddElement(CatalogId catalogId, ParsedElement element)
	{
		_elements.Add(element);
		AddDomainEvent(new ElementParsedDomainEvent(catalogId, element.Properties));
	}
	private void AddErrors(IEnumerable<ParsingError> errors)
	{		
		_errors.AddRange(errors);
	}

	public static ParsingResult Completed(CatalogId catalogId, ScraperId scraperId, DateRange processingTime, IReadOnlyCollection<ParsedElement> elements)
	{
		var result = new ParsingResult(scraperId, ParsingStatus.Completed, processingTime);

		foreach (var element in elements)
			result.AddElement(catalogId, element);

		return result;
	}

	public static ParsingResult ReviewRequired(CatalogId catalogId, ScraperId scraperId, DateRange processingTime, IReadOnlyCollection<ParsedElement> elements, IEnumerable<ParsingError>? errors = null)
	{
		var result = new ParsingResult(scraperId, ParsingStatus.CompletedWithErrors, processingTime);

		foreach (var element in elements)
			result.AddElement(catalogId, element);	

		if(errors?.Any() ?? false)
			result.AddErrors(errors);
		
		return result;
	}

	public static ParsingResult Failed(ScraperId scraperId, ParsingError error, DateRange processingTime)
	{
		var result = new ParsingResult(scraperId, ParsingStatus.Failed, processingTime);		
		result.AddErrors(new List<ParsingError>{error});

		return result;
	}
}
