namespace ITracker.Core.Domain;

public record MoneyToParse : PropertyToParse<MoneyParsed>
{
	private MoneyToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public MoneyToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Money, required)
	{
	}

	public override MoneyParsed? Parse(string? value)
	{
		var regex = new Regex(@"(?<price>\d{1,}((\,|\.)\d{1,2})?)(\s){0,}(?<currency>\€|EUR|\$)?");

		var matches = regex.Match(value.CleanHtmlValue());
		if (matches.Success && decimal.TryParse(matches.Groups["price"].Value.CleanHtmlNumber(), NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
			return new MoneyParsed(Name, new Money(price, matches.Groups["currency"]?.Value ?? "€"));

		return Required ? null : new MoneyParsed(Name, null);
	}
}
