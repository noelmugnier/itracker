namespace ITracker.Core.Application.Tests;

public class CreateCatalogTests
{
	[Fact]
	public async void Should_CreateCatalog()
	{
        var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.Build();
		var command = new CreateBrandCatalogCommand(brandId.Value, "Test Brand", new ProductSchemaDto("Identifier", "Name", "Price", new List<PropertySchemaDto>()));

		var result = await new CreateBrandCatalogCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		var catalog = (await uow.Get<ICatalogRepository>().Get(CatalogId.From(result.Value), CancellationToken.None)).AsT0;
		catalog.Should().NotBeNull();
	}
}
