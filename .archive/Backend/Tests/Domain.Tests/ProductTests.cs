namespace ITracker.Core.Domain.Tests;

public class ProductTests
{
	[Fact]
	public void Should_Raise_ProductCreatedDomainEvent()
	{
		var catalogDefinition = new List<ProductPropertySchema>{
			new ProductPropertySchema(PropertyName.Price, Name.From("Prix"), ValueKind.Text, true, true),
		};

		var product = Product.Create(new List<PropertyParsed>{
			new TextParsed(PropertyName.Identifier, Text.From("12345")),
			new TextParsed(PropertyName.DisplayName, Text.From("test")),
			new MoneyParsed(PropertyName.Price, new Money(5, "EUR")),
		}, CatalogId.New(), catalogDefinition);

		product.DomainEvents.Should().ContainSingle(d => d is ProductCreatedDomainEvent);
	}

	[Fact]
	public void Should_Raise_ProductCreatedDomainEvent_With_Only_TrackedFields()
	{
		var catalogDefinition = new List<ProductPropertySchema>{
			new ProductPropertySchema(PropertyName.Price, Name.From("Prix"), ValueKind.Money, true, true),
			new ProductPropertySchema(PropertyName.From("description"), Name.From("Description"), ValueKind.Text, true, false),
		};

		var product = Product.Create(new List<PropertyParsed>{
			new TextParsed(PropertyName.Identifier, Text.From("12345")),
			new TextParsed(PropertyName.DisplayName, Text.From("test")),
			new TextParsed(PropertyName.From("description"), Text.From("description")),
			new MoneyParsed(PropertyName.Price, new Money(5, "EUR")),
		}, CatalogId.New(), catalogDefinition);

		var domainEvent = product.DomainEvents.OfType<ProductCreatedDomainEvent>().Single();
		domainEvent.Fields.Should().HaveCount(1);
	}

	[Fact]
	public void Should_Raise_ProductTrackedValuesUpdatedDomainEvent()
	{
		var catalogDefinition = new List<ProductPropertySchema>{
			new ProductPropertySchema(PropertyName.Price, Name.From("Prix"), ValueKind.Money, true, true),
		};

		var product = Product.Create(new List<PropertyParsed>{
			new TextParsed(PropertyName.Identifier, Text.From("12345")),
			new TextParsed(PropertyName.DisplayName, Text.From("test")),
			new MoneyParsed(PropertyName.Price, new Money(5, "EUR")),
		}, CatalogId.New(), catalogDefinition);

		product.UpdateWithLastParsedElementData(new List<PropertyParsed>{
			new TextParsed(PropertyName.Identifier, Text.From("12345")),
			new TextParsed(PropertyName.DisplayName, Text.From("test")),
			new MoneyParsed(PropertyName.Price, new Money(10, "EUR")),
		}, catalogDefinition);

		product.DomainEvents.Should().ContainSingle(d => d is ProductTrackedValuesUpdatedDomainEvent);
	}

	[Fact]
	public void Should_NotRaise_ProductTrackedValuesUpdatedDomainEvent()
	{
		var catalogDefinition = new List<ProductPropertySchema>{
			new ProductPropertySchema(PropertyName.Price, Name.From("Prix"), ValueKind.Money, true, true),
		};

		var product = Product.Create(new List<PropertyParsed>{
			new TextParsed(PropertyName.Identifier, Text.From("12345")),
			new TextParsed(PropertyName.DisplayName, Text.From("test")),
			new MoneyParsed(PropertyName.Price, new Money(5, "EUR")),
		}, CatalogId.New(), catalogDefinition);

		product.UpdateWithLastParsedElementData(new List<PropertyParsed>{
			new TextParsed(PropertyName.Identifier, Text.From("12345")),
			new TextParsed(PropertyName.DisplayName, Text.From("test")),
			new MoneyParsed(PropertyName.Price, new Money(5, "EUR")),
		}, catalogDefinition);

		product.DomainEvents.Should().NotContain(d => d is ProductTrackedValuesUpdatedDomainEvent);
	}

	[Fact]
	public void Should_Raise_ProductTrackedValuesUpdatedDomainEvent_With_Only_TrackedFields()
	{
		var catalogDefinition = new List<ProductPropertySchema>{
			new ProductPropertySchema(PropertyName.From("description"), Name.From("Description"), ValueKind.Text, true, false),
			new ProductPropertySchema(PropertyName.Price, Name.From("Prix"), ValueKind.Money, true, true),
		};

		var product = Product.Create(new List<PropertyParsed>{
			new TextParsed(PropertyName.Identifier, Text.From("12345")),
			new TextParsed(PropertyName.DisplayName, Text.From("test")),
			new TextParsed(PropertyName.From("description"), Text.From("super description")),
			new MoneyParsed(PropertyName.Price, new Money(5, "EUR")),
		}, CatalogId.New(), catalogDefinition);

		product.UpdateWithLastParsedElementData(new List<PropertyParsed>{
			new TextParsed(PropertyName.Identifier, Text.From("12345")),
			new TextParsed(PropertyName.DisplayName, Text.From("test")),
			new TextParsed(PropertyName.From("description"), Text.From("super")),
			new MoneyParsed(PropertyName.Price, new Money(10, "EUR")),
		}, catalogDefinition);

		var domainEvent = product.DomainEvents.OfType<ProductTrackedValuesUpdatedDomainEvent>().Single();
		domainEvent.ModifiedFields.Should().HaveCount(1);
	}
}