namespace ITracker.Core.Domain;

public record BrandId
{
	private BrandId(){}

	private BrandId(Guid value)
	{
		if(value == default)
			throw new DomainException(ErrorCode.BrandIdIsInvalid);

		Value = value;
	}

	public Guid Value { get; }

	public static BrandId From(Guid id)
	{
		return new BrandId(id);
	}

	public static BrandId New()
	{
		return new BrandId(Guid.NewGuid());
	}
}
