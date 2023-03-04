namespace ITracker.Core.Application.Tests;

public class ConfigureCatalogSchemaTests
{
	[Fact]
	public async void Should_ConfigureCatalogSchema()
	{
        var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.Build();
		var command = new ConfigureCatalogSchemaCommand(catalogId.Value, new ProductSchemaDto("New Id", "New Name", "New Price", new List<PropertySchemaDto>()));

		var result = await new ConfigureCatalogSchemaCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		var catalog = (await uow.Get<ICatalogRepository>().Get(catalogId, CancellationToken.None)).AsT0;
		catalog.Should().NotBeNull();
		catalog.Schema.Identifier.DisplayName.Should().Be(Name.From("New Id"));
		catalog.Schema.Name.DisplayName.Should().Be(Name.From("New Name"));
		catalog.Schema.Price.DisplayName.Should().Be(Name.From("New Price"));
	}

	[Fact]
	public async void Should_Raise_CatalogProductFieldsUpdatedDomainEvent()
	{
        var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.Build();
		var command = new ConfigureCatalogSchemaCommand(catalogId.Value, new ProductSchemaDto("New Id", "New Name", "New Price", new List<PropertySchemaDto>()));

		var result = await new ConfigureCatalogSchemaCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		uow.RaisedDomainEvents.Should().Contain(rd => rd is CatalogProductFieldsUpdatedDomainEvent);
		uow.RaisedIntegrationEvents.Should().Contain(rd => rd is CatalogProductFieldsUpdatedIntegrationEvent);
	}
}
