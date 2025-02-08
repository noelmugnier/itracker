using ITracker.Core.Domain;

public class ProductHistory : Entity<Guid>
{
	private ProductHistory() : base(Guid.NewGuid()) { }

	private ProductHistory(CatalogId catalogId, ProductId productId, Money price, DateTimeOffset createdOn, IEnumerable<PropertyParsed> modifiedFields)
		: base(Guid.NewGuid())
	{
		Price = price;
		ProductId = productId;
		CatalogId = catalogId;
		CreatedOn = createdOn;
		ModifiedFields = modifiedFields.ToList();
	}

	public static ProductHistory Create(CatalogId catalogId, ProductId productId, Money price, DateTimeOffset createdOn, IEnumerable<PropertyParsed> updatedFields)
	{
		return new ProductHistory(catalogId, productId, price, createdOn, updatedFields);
	}

	public ProductId ProductId { get; }
	public CatalogId CatalogId { get; }
	public Money Price { get; private set; }
	public DateTimeOffset CreatedOn { get; }
	public IReadOnlyCollection<PropertyParsed> ModifiedFields { get; }
}
