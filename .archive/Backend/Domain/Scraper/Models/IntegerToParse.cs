namespace ITracker.Core.Domain;

public record IntegerToParse : PropertyToParse<IntegerParsed>
{
	private IntegerToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public IntegerToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Integer, required)
	{
	}

	public override IntegerParsed? Parse(string? value)
	{
		var regex = new Regex(@"(?<number>\d{1,})");
		var matches = regex.Match(value.CleanHtmlValue());
		if(matches.Success && long.TryParse(matches.Groups["number"].Value, out var result))
			return new IntegerParsed(Name, new IntegerNumber(result));

		return Required ? null : new IntegerParsed(Name,  null);
	}
}
