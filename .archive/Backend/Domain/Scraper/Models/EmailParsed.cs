namespace ITracker.Core.Domain;

public record EmailParsed : PropertyParsed<Email, string>
{
	private EmailParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public EmailParsed(PropertyName name, Email? parsedValue)
		: base(name, ValueKind.Email, parsedValue)
	{
	}
}
