namespace ITracker.Core.Domain;

public record TimeParsed : PropertyParsed<Time, TimeOnly>
{
	private TimeParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public TimeParsed(PropertyName name, Time? parsedValue)
		: base(name, ValueKind.Time, parsedValue)
	{
	}
}
