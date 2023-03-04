namespace ITracker.Core.Domain;

public record IntegerParsed : PropertyParsed<IntegerNumber, long>
{
	private IntegerParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public IntegerParsed(PropertyName name, IntegerNumber? parsedValue)
		: base(name, ValueKind.Integer, parsedValue)
	{
	}
}
