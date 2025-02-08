namespace ITracker.Core.Domain;

public record UrlParsed : PropertyParsed<Url, string>
{
	private UrlParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public UrlParsed(PropertyName name, Url? parsedValue)
		: base(name, ValueKind.Url, parsedValue)
	{
	}
}
