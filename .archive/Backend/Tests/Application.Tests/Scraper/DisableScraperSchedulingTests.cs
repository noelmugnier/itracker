namespace ITracker.Core.Application.Tests;

public class DisableScraperSchedulingTests
{
    [Fact]
    public async void Should_DisableScraperScheduling()
    {
        var uow = new TestDatabaseBuilder()
            .AddBrand(out var brandId)
            .AddCatalog(brandId, out var catalogId)
            .AddScheduledScraper(brandId, catalogId, "0 * * ? * *", out var scraperId)
            .Build();	
        var command = new DisableScraperSchedulingCommand(scraperId.Value);

        var result = await new DisableScraperSchedulingCommandHandler(uow).Handle(command, CancellationToken.None);

        result.Should().BeSuccess();
        var updatedScraper = (await uow.Get<IScraperRepository>().Get(scraperId, CancellationToken.None)).AsT0;
        updatedScraper.SchedulingEnabled.Should().BeFalse();
    }
}
