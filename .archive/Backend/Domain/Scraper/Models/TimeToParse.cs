namespace ITracker.Core.Domain;

public record TimeToParse : PropertyToParse<TimeParsed>
{
	private TimeToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public TimeToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Time, required)
	{
	}

	public override TimeParsed? Parse(string? value)
	{		
		if(TimeOnly.TryParse(value.CleanHtmlValue(), out var date))
			return new TimeParsed(Name, new Time(date));

		return Required ? null : new TimeParsed(Name, null);
	}
}
