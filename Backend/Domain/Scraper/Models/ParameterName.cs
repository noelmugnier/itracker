namespace ITracker.Core.Domain;

public record ParameterName
{	
	private ParameterName(){}
	
	public ParameterName(string value)
	{
		if(value == null)
			throw new DomainException(ErrorCode.ParameterNameIsRequired);

		if(string.IsNullOrWhiteSpace(value))
			throw new DomainException(ErrorCode.ParameterNameCannotBeEmpty);
			
		Value = value;
	}

	public string Value { get; }

	public static ParameterName From(string value)
	{
		return new ParameterName(value);
	}

	public static ParameterName? FromNullable(string? value)
	{
		return value == null ? null : new ParameterName(value);
	}
}

public static class ParameterNameExtensions 
{
	public static string? ToNullable(this ParameterName? parameterName)
	{
		return parameterName?.Value;
	}
}
