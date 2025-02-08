namespace ITracker.Core.Domain;

public record ParsingResultId 
{
	private ParsingResultId(Guid value)
	{
		if(value == default)
			throw new DomainException(ErrorCode.ParsingResultIdIsInvalid);

		Value = value;
	}

	public Guid Value { get; }

	public static ParsingResultId From(Guid id)
	{
		return new ParsingResultId(id);
	}

	public static ParsingResultId New()
	{
		return new ParsingResultId(Guid.NewGuid());
	}
}
