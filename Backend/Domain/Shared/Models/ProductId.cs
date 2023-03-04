namespace ITracker.Core.Domain;

public record ProductId
{
	private ProductId(){}

	private ProductId(string value)
	{
		if(string.IsNullOrWhiteSpace(value))
			throw new DomainException(ErrorCode.ProductIdIsInvalid);

		Value = value;
	}

	public string Value { get; }

	public static ProductId From(string id)
	{
		return new ProductId(id);
	}

	internal static ProductId New()
	{
		return new ProductId(Guid.NewGuid().ToString());
	}
}
