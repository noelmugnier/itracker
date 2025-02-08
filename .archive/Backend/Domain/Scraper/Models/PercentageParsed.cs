namespace ITracker.Core.Domain;

public record PercentageParsed : PropertyParsed<Percentage, decimal>
{
	private PercentageParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public PercentageParsed(PropertyName name, Percentage? parsedValue)
		: base(name, ValueKind.Percentage, parsedValue)
	{
	}

	public override object? RawValue => this.ParsedValue;
}
