namespace ITracker.Core.Domain;

public record ProductSchema(ProductIdPropertySchema Identifier, ProductNamePropertySchema Name, ProductPricePropertySchema Price, IEnumerable<ProductPropertySchema> Fields);

public class ProductSchemaBuilder
{
	private ProductIdPropertySchema _identifier;
	private ProductNamePropertySchema _name;
	private ProductPricePropertySchema _price;
	private List<ProductPropertySchema> _fields = new List<ProductPropertySchema>();

	public ProductSchemaBuilder WithIdentifierProperty(string displayName)
	{
		_identifier = new ProductIdPropertySchema(Name.From(displayName));
		return this;
	}

	public ProductSchemaBuilder WithNameProperty(string displayName)
	{
		_name = new ProductNamePropertySchema(Name.From(displayName));
		return this;
	}

	public ProductSchemaBuilder WithPriceProperty(string displayName)
	{
		_price = new ProductPricePropertySchema(Name.From(displayName));
		return this;
	}

	public ProductSchemaBuilder WithFields(IEnumerable<ProductPropertySchema> fields)
	{
		foreach (var field in fields)
			AddField(field.PropertyName.Value, field.DisplayName.Value, field.ValueType, field.Required, field.Tracked);
			
		return this;
	}

	public ProductSchemaBuilder AddField(string propertyName, string displayName, ValueKind valueType, bool required = true, bool tracked = false)
	{
		if (_fields.Any(f => f.PropertyName.Value == propertyName))
			throw new InvalidOperationException("Duplicate field property name found");

		_fields.Add(new ProductPropertySchema(PropertyName.From(propertyName), Name.From(displayName), valueType, required, tracked));
		return this;
	}

	public ProductSchema Build()
	{
		return new ProductSchema(_identifier, _name, _price, _fields);
	}
}
