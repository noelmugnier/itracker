namespace ITracker.Core.Domain;

public record RatingParsed : PropertyParsed<Rating, decimal>
{
	private RatingParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public RatingParsed(PropertyName name, Rating? parsedValue)
		: base(name, ValueKind.Rating, parsedValue)
	{
	}

	public override object? RawValue => this.ParsedValue;
}
