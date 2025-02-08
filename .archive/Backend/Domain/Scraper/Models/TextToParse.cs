namespace ITracker.Core.Domain;

public record TextToParse : PropertyToParse<TextParsed>
{
	private TextToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public TextToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Text, required)
	{
	}

	public override TextParsed? Parse(string? value)
	{
		if(!string.IsNullOrWhiteSpace(value.CleanHtmlValue()))		
			return new TextParsed(Name, new Text(value.CleanHtmlValue()));

		return Required ? null : new TextParsed(Name, new Text(string.Empty));
	}
}
