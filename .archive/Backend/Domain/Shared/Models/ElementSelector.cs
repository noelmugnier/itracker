namespace ITracker.Core.Domain;

public record ElementSelector
{
	private ElementSelector(){}
	
	private ElementSelector(string value)
	{
		if(value is null)
			throw new DomainException(ErrorCode.ElementSelectorIsRequired);

		if(string.IsNullOrWhiteSpace(value))
			throw new DomainException(ErrorCode.ElementSelectorCannotBeEmpty);
			
		Value = value;
	}

	public string Value { get; }

	public static ElementSelector From(string value)
	{
		return new ElementSelector(value);
	}
}