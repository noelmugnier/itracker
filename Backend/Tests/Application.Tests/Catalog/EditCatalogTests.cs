namespace ITracker.Core.Application.Tests;

public class EditCatalogTests
{
	[Fact]
	public async void Should_UpdateCatalog()
	{
        var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.Build();
		var command = new EditCatalogCommand(catalogId.Value, "New test Brand");

		var result = await new EditCatalogCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		var catalog = (await uow.Get<ICatalogRepository>().Get(catalogId, CancellationToken.None)).AsT0;
		catalog.Should().NotBeNull();
		catalog.Name.Should().Be(Name.From("New test Brand"));
	}
}