namespace ITracker.Core.Domain;

public record DecimalParsed : PropertyParsed<DecimalNumber, decimal>
{
	private DecimalParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public DecimalParsed(PropertyName name, DecimalNumber? parsedValue)
		: base(name, ValueKind.Decimal, parsedValue)
	{
	}
}
