using ITracker.Core.Domain;

public class Catalog : Entity<CatalogId>
{
	private Catalog() : base(CatalogId.New()) { }

	private Catalog(BrandId brandId, Name name, ProductSchema productSchema) : base(CatalogId.New())
	{
		BrandId = brandId;
		Name = name;
		Schema = productSchema;
		CreatedOn = DateTimeOffset.UtcNow;
		UpdatedOn = DateTimeOffset.UtcNow;
	}

	public static Catalog Create(BrandId brandId, Name name, ProductSchema schema)
	{
		var catalog = new Catalog(brandId, name, schema);
		catalog.AddDomainEvent(new CatalogCreatedDomainEvent(catalog.Id));

		return catalog;
	}

	public Name Name { get; set; }
	public ProductSchema Schema { get; private set; }
	public DateTimeOffset CreatedOn { get; } = DateTimeOffset.UtcNow;
	public DateTimeOffset UpdatedOn { get; private set; } = DateTimeOffset.UtcNow;
	public BrandId BrandId { get; }

	public IEnumerable<PropertyFieldSchema> GetAllSchemaFields()
	{
		var fields = new List<PropertyFieldSchema>();
		fields.Add(new PropertyFieldSchema(Schema.Identifier.PropertyName, Schema.Identifier.ValueType, Schema.Identifier.Required, Schema.Identifier.Tracked));
		fields.Add(new PropertyFieldSchema(Schema.Name.PropertyName, Schema.Name.ValueType, Schema.Name.Required, Schema.Name.Tracked));
		fields.Add(new PropertyFieldSchema(Schema.Price.PropertyName, Schema.Price.ValueType, Schema.Price.Required, Schema.Price.Tracked));

		foreach (var property in Schema.Fields)
			fields.Add(new PropertyFieldSchema(property.PropertyName, property.ValueType, property.Required, property.Tracked));

		return fields;
	}

	public IEnumerable<ProductPropertySchema> GetProductProperties()
	{
		var fields = new List<ProductPropertySchema>();
		fields.Add(new ProductIdPropertySchema(Schema.Identifier.DisplayName));
		fields.Add(new ProductNamePropertySchema(Schema.Name.DisplayName));
		fields.Add(new ProductPricePropertySchema(Schema.Price.DisplayName));

		foreach (var property in Schema.Fields)
			fields.Add(new ProductPropertySchema(property.PropertyName, property.DisplayName, property.ValueType, property.Required, property.Tracked));

		return fields;
	}

	public void ConfigureSchema(ProductSchema productSchema)
	{
		Schema = productSchema;
		UpdatedOn = DateTimeOffset.UtcNow;

		AddDomainEvent(new CatalogProductFieldsUpdatedDomainEvent(Id, GetAllSchemaFields()));
	}
}
