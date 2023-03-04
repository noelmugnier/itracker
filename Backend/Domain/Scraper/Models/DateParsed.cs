namespace ITracker.Core.Domain;

public record DateParsed : PropertyParsed<Date, DateOnly>
{
	private DateParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}

	public DateParsed(PropertyName name, Date? parsedValue)
		: base(name, ValueKind.Date, parsedValue)
	{
	}
}
