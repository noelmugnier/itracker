namespace ITracker.Core.Domain;

public record TextParsed : PropertyParsed<Text, string>
{
	private TextParsed() 
		: base(PropertyName.Empty(), ValueKind.Unknown, null) {}
		
	public TextParsed(PropertyName name, Text? parsedValue)
		: base(name, ValueKind.Text, parsedValue)
	{
	}
}
