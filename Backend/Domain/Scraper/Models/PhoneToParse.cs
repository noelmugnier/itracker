namespace ITracker.Core.Domain;

public record PhoneToParse : PropertyToParse<PhoneParsed>
{
	private PhoneToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public PhoneToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Phone, required)
	{
	}

	public override PhoneParsed? Parse(string? value)
	{
		throw new NotImplementedException();
	}
}
