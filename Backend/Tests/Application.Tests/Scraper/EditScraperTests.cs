namespace ITracker.Core.Application.Tests;

public class EditScraperTests
{
	[Fact]
	public async void Should_UpdateScraper()
	{
        var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.AddScraper(brandId, catalogId, out var scraperId)
			.Build();
		var command = new EditScraperCommand(scraperId.Value, "New test scraper", "https://www.test.com/page");

		var result = await new EditScraperCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		var scraper = (await uow.Get<IScraperRepository>().Get(scraperId, CancellationToken.None)).AsT0;
		scraper.Should().NotBeNull();
		scraper.Name.Should().Be(Name.From("New test scraper"));
		scraper.Uri.Should().Be(ScraperUri.From("https://www.test.com/page"));
	}
}