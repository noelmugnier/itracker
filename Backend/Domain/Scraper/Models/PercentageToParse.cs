namespace ITracker.Core.Domain;

public record PercentageToParse : PropertyToParse<PercentageParsed>
{
	private PercentageToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public PercentageToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Percentage, required)
	{
	}

	public override PercentageParsed? Parse(string? value)
	{
		var percentRegex = new Regex(@"(?<percent>\d{1,}((\,|\.)\d{1,2})?)(\s){0,}\%");
		var matches = percentRegex.Match(value.CleanHtmlValue());
		if (matches.Success && decimal.TryParse(matches.Groups["percent"].Value.CleanHtmlNumber(), NumberStyles.Any, CultureInfo.InvariantCulture, out var percentage))
			return new PercentageParsed(Name, new Percentage(percentage / 100));

		return Required ? null : new PercentageParsed(Name, null);	
	}
}
