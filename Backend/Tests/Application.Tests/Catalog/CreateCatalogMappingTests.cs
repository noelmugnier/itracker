namespace ITracker.Core.Application.Tests;

public class CreateCatalogMappingTests
{
	[Fact]
	public async void Should_CreateMapping()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brand1Id, true)
			.AddCatalog(brand1Id, out var catalog1Id)
			.AddProduct(catalog1Id, out var product1Id)
			.AddBrand(out var brand2Id)
			.AddCatalog(brand2Id, out var catalog2Id)
			.AddProduct(catalog2Id, out var product2Id)
			.Build();

		var command = new CreateCatalogsMappingCommand(brand1Id.Value, new CatalogsMappingDto(catalog1Id.Value, catalog2Id.Value, new List<ProductMappingDto>
		{
			new ProductMappingDto(product1Id.Value, product2Id.Value)
		}));

		var result = await new CreateCatalogsMappingCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();
	}

	[Fact]
	public async void Should_Fail_To_CreateMapping_If_SourceCatalogNotInDefaultBrand()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brand1Id)
			.AddCatalog(brand1Id, out var catalog1Id)
			.AddProduct(catalog1Id, out var product1Id)
			.AddBrand(out var brand2Id)
			.AddCatalog(brand2Id, out var catalog2Id)
			.AddProduct(catalog2Id, out var product2Id)
			.Build();

		var command = new CreateCatalogsMappingCommand(brand1Id.Value, new CatalogsMappingDto(catalog1Id.Value, catalog2Id.Value, new List<ProductMappingDto>
		{
			new ProductMappingDto(product1Id.Value, product2Id.Value)
		}));

		var result = await new CreateCatalogsMappingCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeFailure();
	}
}
