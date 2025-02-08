namespace ITracker.Core.Domain;

public record RatingToParse : PropertyToParse<RatingParsed>
{
	private RatingToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public RatingToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Rating, required)
	{
	}

	public override RatingParsed? Parse(string? value)
	{
		throw new NotImplementedException();
	}
}
