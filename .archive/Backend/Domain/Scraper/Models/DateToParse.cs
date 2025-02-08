namespace ITracker.Core.Domain;

public record DateToParse : PropertyToParse<DateParsed>
{
	private DateToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public DateToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Date, required)
	{
	}

	public override DateParsed? Parse(string? value)
	{
		if(DateOnly.TryParse(value.CleanHtmlValue(), out var date))
			return new DateParsed(Name, new Date(date));
		
		return Required ? null : new DateParsed(Name, null);
	}
}
