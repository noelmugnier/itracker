namespace ITracker.Core.Domain;

public record PhoneParsed : PropertyParsed<Phone, string>
{
	private PhoneParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public PhoneParsed(PropertyName name, Phone? parsedValue)
		: base(name, ValueKind.Phone, parsedValue)
	{
	}
}
