namespace ITracker.Core.Application.Tests;

public class ProductTrackedValuesUpdatedDomainEventTests
{
	[Fact]
	public async void Should_AddProductHistory()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.AddProduct(catalogId, out var productId)
			.Build();

		var command = new ProductTrackedValuesUpdatedDomainEvent(catalogId, productId, new Money(10, "€"), new List<PropertyParsed>{});
		var result = await new ProductTrackedValuesUpdatedDomainEventHandler(uow).HandleEvent(command, CancellationToken.None);

		result.Should().BeSuccess();
		var productHistory = (await uow.Get<IProductHistoryRepository>().GetProductHistory(catalogId, productId, CancellationToken.None)).Value;
		productHistory.Should().HaveCount(1);
	}
}