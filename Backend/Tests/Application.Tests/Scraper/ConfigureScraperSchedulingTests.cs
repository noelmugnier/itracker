namespace ITracker.Core.Application.Tests;

public class ConfigureScraperSchedulingTests
{
    [Fact]
    public async void Should_EnableScraperScheduling()
    {
        var uow = new TestDatabaseBuilder()
            .AddBrand(out var brandId)
            .AddCatalog(brandId, out var catalogId)
            .AddScraper(brandId, catalogId, out var scraperId)
            .Build();
        var command = new ConfigureScraperSchedulingCommand(scraperId.Value, new SchedulingDto("0 * * ? * *"));
        
        var result = await new ConfigureScraperSchedulingCommandHandler(uow).Handle(command, CancellationToken.None);

        result.Should().BeSuccess();
        var updatedScraper = (await uow.Get<IScraperRepository>().Get(scraperId, CancellationToken.None)).AsT0;
        updatedScraper.SchedulingEnabled.Should().BeTrue();
    }
}
