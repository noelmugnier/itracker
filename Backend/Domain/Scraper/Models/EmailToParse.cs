namespace ITracker.Core.Domain;

public record EmailToParse : PropertyToParse<EmailParsed>
{
	private EmailToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public EmailToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Email, required)
	{
	}

	public override EmailParsed? Parse(string? value)
	{
		throw new NotImplementedException();
	}
}
