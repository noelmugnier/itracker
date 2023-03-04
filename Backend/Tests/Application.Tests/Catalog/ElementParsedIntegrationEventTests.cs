namespace ITracker.Core.Application.Tests;

public class ElementParsedIntegrationEventTests
{
	[Fact]
	public async void Should_InsertProduct()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.Build();

		var integrationEvent = new ElementParsedIntegrationEvent(catalogId.Value, new List<PropertyParsed>
		{
			new TextParsed(PropertyName.Identifier, new Text("12345")),
			new TextParsed(PropertyName.DisplayName, new Text("super name")),
			  new MoneyParsed(PropertyName.Price, new Money(125, "€")),
		});

		var result = await new ElementParsedIntegrationEventHandler(uow).HandleEvent(integrationEvent, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		var product = (await uow.Get<IProductRepository>().Get(ProductId.From("12345"), CancellationToken.None)).AsT0;
		product.Should().NotBeNull();
		product.Id.Should().Be(ProductId.From("12345"));
		product.Name.Should().Be(Name.From("super name"));
	}

	[Fact]
	public async void Should_InsertProductCreatedEvent()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.Build();

		var integrationEvent = new ElementParsedIntegrationEvent(catalogId.Value, new List<PropertyParsed>
		{
			new TextParsed(PropertyName.Identifier, new Text("12345")),
			new TextParsed(PropertyName.DisplayName, new Text("super name")),
			  new MoneyParsed(PropertyName.Price, new Money(125, "€")),
	});

		var result = await new ElementParsedIntegrationEventHandler(uow).HandleEvent(integrationEvent, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		uow.RaisedDomainEvents.Should().ContainSingle(rd => rd is ProductCreatedDomainEvent);
	}

	[Fact]
	public async void Should_UpdateProduct()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.AddProduct(catalogId, out var productId)
			.Build();

		var integrationEvent = new ElementParsedIntegrationEvent(catalogId.Value, new List<PropertyParsed>
		{
			new TextParsed(PropertyName.Identifier, new Text(productId.Value)),
			new TextParsed(PropertyName.DisplayName, new Text("name updated")),
		  new MoneyParsed(PropertyName.Price, new Money(125, "€")),
	});

		var result = await new ElementParsedIntegrationEventHandler(uow).HandleEvent(integrationEvent, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		var product = (await uow.Get<IProductRepository>().Get(productId, CancellationToken.None)).AsT0;
		product.Should().NotBeNull();
		product.Name.Should().Be(Name.From("name updated"));
		product.Price.Should().Be(new Money(125, "€"));
	}

	[Fact]
	public async void Should_NotRaiseTrackedValuesUpdatedEvent_When_NoTrackedValuesChanged()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.AddProduct(catalogId, out var productId, new List<PropertyParsed>
			{
				new TextParsed(PropertyName.Identifier, new Text("12345")),
				new TextParsed(PropertyName.DisplayName, new Text("super name")),
				new MoneyParsed(PropertyName.Price, new Money(10, "€"))
			})
			.Build();

		var integrationEvent = new ElementParsedIntegrationEvent(catalogId.Value, new List<PropertyParsed>
		{
			new TextParsed(PropertyName.Identifier, new Text(productId.Value)),
			new TextParsed(PropertyName.DisplayName, new Text("super name")),
			new MoneyParsed(PropertyName.Price, new Money(10, "€"))
		});

		var result = await new ElementParsedIntegrationEventHandler(uow).HandleEvent(integrationEvent, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		uow.RaisedDomainEvents.Should().BeEmpty();
	}

	[Fact]
	public async void Should_RaiseTrackedValuesUpdatedEvent_When_TrackedValuesChanged()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.AddProduct(catalogId, out var productId, new List<PropertyParsed>
			{
				new TextParsed(PropertyName.Identifier, new Text("12345")),
				new TextParsed(PropertyName.DisplayName, new Text("super name")),
				new MoneyParsed(PropertyName.Price, new Money(5, "€")),
			})
			.Build();

		var integrationEvent = new ElementParsedIntegrationEvent(catalogId.Value, new List<PropertyParsed>
		{
			new TextParsed(PropertyName.Identifier, new Text(productId.Value)),
			new TextParsed(PropertyName.DisplayName, new Text("super name")),
			new MoneyParsed(PropertyName.Price, new Money(10, "€")),
	});

		var result = await new ElementParsedIntegrationEventHandler(uow).HandleEvent(integrationEvent, CancellationToken.None);

		result.IsSuccess.Should().BeTrue();
		uow.RaisedDomainEvents.Should().ContainSingle(rd => rd is ProductTrackedValuesUpdatedDomainEvent);
	}
}
