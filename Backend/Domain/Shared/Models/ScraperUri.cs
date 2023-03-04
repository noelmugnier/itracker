namespace ITracker.Core.Domain;

public record ScraperUri
{
	private ScraperUri(){}

	private ScraperUri(Uri value)
	{
		if(value == default || !value.IsAbsoluteUri)
			throw new DomainException(ErrorCode.ScraperUriIsInvalid);

		Value = value;
	}

	public Uri Value { get; }

	public static ScraperUri From(string url)
	{
		if(!Uri.TryCreate(url, UriKind.Absolute, out var uri))
			throw new DomainException(ErrorCode.ScraperUriIsInvalid);

		return new ScraperUri(uri);
	}

	public static ScraperUri From(Uri uri)
	{
		return new ScraperUri(uri);
	}
}
