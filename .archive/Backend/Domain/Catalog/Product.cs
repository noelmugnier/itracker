namespace ITracker.Core.Domain;

public class Product : Entity<ProductId>
{
	private readonly List<PropertyParsed> _fields = new();

	private Product() : this(ProductId.New(), Name.From("test"), new Money(0, "€"), new List<PropertyParsed>(), CatalogId.New()) { }

	private Product(ProductId productId, Name name, Money price, IEnumerable<PropertyParsed> fields, CatalogId catalogId)
		: base(productId)
	{
		CatalogId = catalogId;
		Name = name;
		Price = price;
		CreatedOn = DateTimeOffset.UtcNow;
		UpdatedOn = DateTimeOffset.UtcNow;

		_fields.AddRange(fields.Where(f => f.Name != PropertyName.Identifier && f.Name != PropertyName.DisplayName));
	}

	public static Product Create(IEnumerable<PropertyParsed> fields, CatalogId catalogId, IEnumerable<ProductPropertySchema> schemaProperties)
	{
		var productId = GetIdentifierFromFields(fields);
		var name = GetDisplayNameFromFields(fields);
		var price = GetPriceFromFields(fields);

		var product = new Product(productId, name, price, fields, catalogId);

		var trackedFields = schemaProperties.Where(sp => sp.Tracked).Select(sp => sp.PropertyName);
		product.AddDomainEvent(new ProductCreatedDomainEvent(catalogId, product.Id, product.Price, fields.Where(f => trackedFields.Contains(f.Name))));

		return product;
	}

	public static ProductId GetIdentifierFromFields(IEnumerable<PropertyParsed> fields)
	{
		var id = fields.Single(pp => pp.Name == PropertyName.Identifier) as TextParsed;
		return ProductId.From(id!.ParsedValue!.Value!);
	}

	public static Name GetDisplayNameFromFields(IEnumerable<PropertyParsed> fields)
	{
		var name = fields.Single(pp => pp.Name == PropertyName.DisplayName) as TextParsed;
		return Name.From(name!.ParsedValue!.Value!);
	}

	public static Money GetPriceFromFields(IEnumerable<PropertyParsed> fields)
	{
		var money = fields.Single(pp => pp.Name == PropertyName.Price) as MoneyParsed;
		return new Money(money!.ParsedValue!.Value!, money!.ParsedValue!.Currency!);
	}

	public Name Name { get; private set; }
	public Money Price { get; private set; }
	public DateTimeOffset CreatedOn { get; } = DateTimeOffset.UtcNow;
	public DateTimeOffset UpdatedOn { get; private set; } = DateTimeOffset.UtcNow;
	public IReadOnlyCollection<PropertyParsed> Fields => _fields;
	public CatalogId CatalogId { get; }

	public void UpdateWithLastParsedElementData(IEnumerable<PropertyParsed> newFields, IEnumerable<ProductPropertySchema> schemaProperties)
	{
		var newName = GetDisplayNameFromFields(newFields);
		if (Name != newName)
			Name = newName;

		var newPrice = GetPriceFromFields(newFields);
		if (Price != newPrice)
			Price = newPrice;

		var updatedFields = new List<PropertyParsed>();
		foreach (var schemaField in schemaProperties)
		{
			var existingField = _fields.SingleOrDefault(f => f.Name == schemaField.PropertyName);
			var newField = newFields.SingleOrDefault(f => f.Name == schemaField.PropertyName);

			if (existingField is not null)
				_fields.Remove(existingField);

			if (newField is not null)
				_fields.Add(newField);

			if (!schemaField.Tracked)
				continue;

			if (existingField is not null && newField is not null && !existingField.Equals(newField))
				updatedFields.Add(newField);

			if (existingField is null && newField is not null)
				updatedFields.Add(newField);

			if (existingField is not null && newField is null)
				updatedFields.Add(existingField.AsNullProperty());
		}

		UpdatedOn = DateTimeOffset.UtcNow;

		if (updatedFields.Any())
			AddDomainEvent(new ProductTrackedValuesUpdatedDomainEvent(CatalogId, Id, Price, updatedFields));
	}
}
