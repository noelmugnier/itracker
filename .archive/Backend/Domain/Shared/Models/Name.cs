namespace ITracker.Core.Domain;

public record Name : ValueObject<string>
{
	private Name():base(string.Empty){}
	
	public Name(string value) : base(value)
	{
		if(value == null)
			throw new DomainException(ErrorCode.NameIsRequired);

		if(string.IsNullOrWhiteSpace(value))
			throw new DomainException(ErrorCode.NameCannotBeEmpty);
	}

	public static Name From(string value)
	{
		return new Name(value);
	}

	public static Name? FromNullable(string? value)
	{
		return value == null ? null : new Name(value);
	}
}

public static class NameExtensions 
{
	public static string? ToNullable(this Name? name)
	{
		return name?.Value;
	}
}
