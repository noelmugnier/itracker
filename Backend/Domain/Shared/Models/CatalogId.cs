namespace ITracker.Core.Domain;

public record CatalogId
{
	private CatalogId(){}

	private CatalogId(Guid value)
	{
		if(value == Guid.Empty)
			throw new DomainException(ErrorCode.CatalogIdIsInvalid);

		Value = value;
	}

	public Guid Value { get; }

	public static CatalogId From(Guid id)
	{
		return new CatalogId(id);
	}

	public static CatalogId New()
	{
		return new CatalogId(Guid.NewGuid());
	}
}
