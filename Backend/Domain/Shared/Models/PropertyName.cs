namespace ITracker.Core.Domain;

public record PropertyName
{	
	public static PropertyName Identifier => PropertyName.From("identifier");
	public static PropertyName DisplayName => PropertyName.From("display_name");
	public static PropertyName Price => PropertyName.From("price");

	private PropertyName(){}
	
	public PropertyName(string value, bool check)
	{
		if(check){
			if(value == null)
				throw new DomainException(ErrorCode.PropertyNameIsRequired);

			if(string.IsNullOrWhiteSpace(value))
				throw new DomainException(ErrorCode.PropertyNameCannotBeEmpty);
		}
			
		Value = value;
	}

	public string Value { get; }

	public static PropertyName From(string value)
	{
		return new PropertyName(value, true);
	}

	public static PropertyName Empty()
	{
		return new PropertyName(string.Empty, false);
	}
}
