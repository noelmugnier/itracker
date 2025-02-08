namespace ITracker.Core.Domain;

public record MoneyParsed : PropertyParsed<Money, decimal>
{
	private MoneyParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public MoneyParsed(PropertyName name, Money? parsedValue)
		: base(name, ValueKind.Money, parsedValue)
	{
	}

	public override object? RawValue => this.ParsedValue;
}
