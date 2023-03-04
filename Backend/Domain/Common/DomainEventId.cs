namespace ITracker.Core.Domain;

public record DomainEventId
{	
	private DomainEventId(){}
	
	private DomainEventId(Guid value)
	{
		if(value == default)
			throw new DomainException(ErrorCode.EventIdIsInvalid);

		Value = value;
	}

	public Guid Value { get; }

	public static DomainEventId New()
	{
		return new DomainEventId(Guid.NewGuid());
	}
}
