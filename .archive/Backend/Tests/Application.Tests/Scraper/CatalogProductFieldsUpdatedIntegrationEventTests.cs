namespace ITracker.Core.Application.Tests;

public class CatalogProductFieldsUpdatedIntegrationEventTests
{
	[Fact]
	public async void Should_UpdateCatalogRelatedScraperStatusToReviewRequired()
	{		
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalog1Id)
			.AddScraper(brandId, catalog1Id, out var catalog1Scraper1Id)
			.AddScraper(brandId, catalog1Id, out var catalog1Scraper2Id)
			.AddCatalog(brandId, out var catalog2Id)
			.AddScraper(brandId, catalog2Id, out var catalog2ScraperId)
			.Build();
		var @event = new CatalogProductFieldsUpdatedIntegrationEvent(catalog1Id.Value, new List<PropertyFieldSchema>());
		
		var result = await new CatalogProductFieldsUpdatedIntegrationEventHandler(uow).HandleEvent(@event, CancellationToken.None);

		result.Should().BeSuccess();
		var catalog1Scraper1 = (await uow.Get<IScraperRepository>().Get(catalog1Scraper1Id, CancellationToken.None)).AsT0;
		catalog1Scraper1.Status.Should().Be(ScraperStatus.ReviewRequired);
		var catalog1Scraper2 = (await uow.Get<IScraperRepository>().Get(catalog1Scraper2Id, CancellationToken.None)).AsT0;
		catalog1Scraper2.Status.Should().Be(ScraperStatus.ReviewRequired);
		var catalog2Scraper = (await uow.Get<IScraperRepository>().Get(catalog2ScraperId, CancellationToken.None)).AsT0;
		catalog2Scraper.Status.Should().Be(ScraperStatus.Active);
	}
}