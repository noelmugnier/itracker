namespace ITracker.Core.Domain;

public record ParsedContent 
{
	private ParsedContent(IEnumerable<ParsedElement> elements, IEnumerable<IError>? errors = null)
	{
		Elements = elements.ToList();
		Errors = errors?.ToList() ?? new List<IError>();
	}

	public static ParsedContent Empty()
	{
		return new(new List<ParsedElement>());
	}

	public static ParsedContent Complete(IEnumerable<ParsedElement> elements)
	{
		return new(elements);
	}

	public static ParsedContent CompleteWithErrors(IEnumerable<ParsedElement> elements, IEnumerable<IError> errors)
	{
		return new(elements, errors);
	}
	
	public IReadOnlyCollection<ParsedElement> Elements { get; }
	public IReadOnlyCollection<IError> Errors { get; }
}

