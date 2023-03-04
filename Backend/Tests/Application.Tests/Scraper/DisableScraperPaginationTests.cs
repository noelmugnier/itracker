namespace ITracker.Core.Application.Tests;

public class DisableScraperPaginationTests
{
    [Fact]
    public async void Should_DisableScraperPagination()
    {
        var uow = new TestDatabaseBuilder()
            .AddBrand(out var brandId)
            .AddCatalog(brandId, out var catalogId)
            .AddPaginatedScraper(brandId, catalogId, 1, out var scraperId)
            .Build();	
        var command = new DisableScraperPaginationCommand(scraperId.Value);
        
        var result = await new DisableScraperPaginationCommandHandler(uow).Handle(command, CancellationToken.None);

        result.Should().BeSuccess();
        var updatedScraper = (await uow.Get<IScraperRepository>().Get(scraperId, CancellationToken.None)).AsT0;
        updatedScraper.PaginationEnabled.Should().BeFalse();
    }
}
