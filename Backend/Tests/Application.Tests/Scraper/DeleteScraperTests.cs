namespace ITracker.Core.Application.Tests;

public class DeleteScraperTests
{
	[Fact]
	public async void Should_DeleteScraper()
	{
        var uow = new TestDatabaseBuilder()
            .AddBrand(out var brandId)
            .AddCatalog(brandId, out var catalogId)
            .AddScraper(brandId, catalogId, out var scraperId)
            .Build();	
		var command = new DeleteScraperCommand(scraperId.Value);

		var result = await new DeleteScraperCommandHandler(uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();		
		var scraperDeleted = (await uow.Get<IScraperRepository>().Get(scraperId, CancellationToken.None)).AsT1;
		scraperDeleted.Should().NotBeNull();
	}
}