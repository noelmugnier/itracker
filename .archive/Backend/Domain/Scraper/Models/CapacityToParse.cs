namespace ITracker.Core.Domain;

public record CapacityToParse : PropertyToParse<CapacityParsed>
{
	private CapacityToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public CapacityToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Capacity, required)
	{
	}

	public override CapacityParsed? Parse(string? value)
	{
		var capacityRegex = new Regex(@"(?<capacity>\d{1,}((\,|\.)\d{1,2})?)(\s){0,}(?<unit>KG|kg|Kg|kG|gr|Gr|gR|g|Ml|ml|mL|cL|cl|Cl|dL|dl|Dl|L|l)?");

		var matches = capacityRegex.Match(value.CleanHtmlValue());
		if (matches.Success && decimal.TryParse(matches.Groups["capacity"].Value.CleanHtmlNumber(), NumberStyles.Any, CultureInfo.InvariantCulture, out var capacity))
			return new CapacityParsed(Name, new Capacity(capacity, matches.Groups["unit"]?.Value ?? ""));
		
		return Required ? null : new CapacityParsed(Name, null);
	}
}
