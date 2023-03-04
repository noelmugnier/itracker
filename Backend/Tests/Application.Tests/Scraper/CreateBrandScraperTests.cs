namespace ITracker.Core.Application.Tests;

public class CreateCatalogScraperTests
{
    [Fact]
    public async void Should_InsertScraper_With_DisabledPagination()
    {
        var uow = new TestDatabaseBuilder()
            .AddBrand(out var brandId)
            .AddCatalog(brandId, out var catalogId)
            .Build();    
        var scraperFieldsRetriever = new FakeScraperFieldsRetriever(uow);
        var command = new CreateCatalogScraperCommand(brandId.Value, catalogId.Value, "Test Scraper", "https://example.com", new ParserDto("root", new List<PropertySelectorDto>{new PropertySelectorDto(PropertyName.Identifier.Value, "h1"), new PropertySelectorDto(PropertyName.DisplayName.Value, "h1"), new PropertySelectorDto(PropertyName.Price.Value, "h1")}));
                
        var result = await new CreateCatalogScraperCommandHandler(scraperFieldsRetriever, uow).Handle(command, CancellationToken.None);
        
        result.Should().BeSuccess();
        var createdScraper = (await uow.Get<IScraperRepository>().Get(ScraperId.From(result.Value), CancellationToken.None)).AsT0;
        createdScraper.Should().NotBeNull();
        createdScraper.PaginationEnabled.Should().BeFalse();
    }

    [Fact]
    public async void Should_InsertScraper_With_DefaultName()
    {
        var uow = new TestDatabaseBuilder()
            .AddBrand(out var brandId)
            .AddCatalog(brandId, out var catalogId)
            .Build();    
        var scraperFieldsRetriever = new FakeScraperFieldsRetriever(uow);
        var command = new CreateCatalogScraperCommand(brandId.Value, catalogId.Value, null, "http://example.com/category/test_ok-123?param=1", new ParserDto("root", new List<PropertySelectorDto>{new PropertySelectorDto(PropertyName.Identifier.Value, "h1"), new PropertySelectorDto(PropertyName.DisplayName.Value, "h1"), new PropertySelectorDto(PropertyName.Price.Value, "h1")}));
                
        var result = await new CreateCatalogScraperCommandHandler(scraperFieldsRetriever, uow).Handle(command, CancellationToken.None);
        
        result.Should().BeSuccess();
        var createdScraper = (await uow.Get<IScraperRepository>().Get(ScraperId.From(result.Value), CancellationToken.None)).AsT0;
        createdScraper.Should().NotBeNull();
        createdScraper.Name.Should().Be(Name.From("category - test ok 123"));
    }
}
