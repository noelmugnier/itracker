namespace ITracker.Core.Domain;

public record ScraperId
{
	private ScraperId(){}

	private ScraperId(Guid value)
	{
		if(value == default)
			throw new DomainException(ErrorCode.ScraperIdIsInvalid);

		Value = value;
	}

	public Guid Value { get; }

	public static ScraperId From(Guid id)
	{
		return new ScraperId(id);
	}

	public static ScraperId New()
	{
		return new ScraperId(Guid.NewGuid());
	}
}
