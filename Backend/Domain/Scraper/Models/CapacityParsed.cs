namespace ITracker.Core.Domain;

public record CapacityParsed : PropertyParsed<Capacity, decimal>
{
	private CapacityParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}

	public CapacityParsed(PropertyName name, Capacity? parsedValue)
		: base(name, ValueKind.Capacity, parsedValue)
	{
	}

	public override object? RawValue => this.ParsedValue;
}
