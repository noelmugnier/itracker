namespace ITracker.Core.Application.Tests;

public class DeleteCatalogTests
{
	[Fact]
	public async void Should_DeleteCatalog()
	{
        var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.Build();
		var command = new DeleteCatalogCommand(catalogId.Value);

		var result = await new DeleteCatalogCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		var catalog = (await uow.Get<ICatalogRepository>().Get(catalogId, CancellationToken.None)).AsT1;
		catalog.Should().NotBeNull();
	}
}