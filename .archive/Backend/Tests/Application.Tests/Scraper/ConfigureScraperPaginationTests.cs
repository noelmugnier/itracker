namespace ITracker.Core.Application.Tests;

public class ConfigureScraperPaginationTests
{
    [Fact]
    public async void Should_EnableScraperPagination()
    {
        var uow = new TestDatabaseBuilder()
            .AddBrand(out var brandId)
            .AddCatalog(brandId, out var catalogId)
            .AddScraper(brandId, catalogId, out var scraperId)
            .Build();
        var command = new ConfigureScraperPaginationCommand(scraperId.Value, new PaginationDto("page", 10, 10));        

        var result = await new ConfigureScraperPaginationCommandHandler(uow).Handle(command, CancellationToken.None);

        result.Should().BeSuccess();
        var updatedScraper = (await uow.Get<IScraperRepository>().Get(scraperId, CancellationToken.None)).AsT0;
        updatedScraper.PaginationEnabled.Should().BeTrue();
    }
}
