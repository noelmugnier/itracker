namespace ITracker.Core.Domain;

public record DecimalToParse : PropertyToParse<DecimalParsed>
{
	private DecimalToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public DecimalToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Decimal, required)
	{
	}

	public override DecimalParsed? Parse(string? value)
	{
		if(decimal.TryParse(value.CleanHtmlNumber(), NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var result))
			return new DecimalParsed(Name, new DecimalNumber(result));

		return Required ? null : new DecimalParsed(Name,  null);
	}
}
