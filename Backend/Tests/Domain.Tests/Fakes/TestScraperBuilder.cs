namespace ITracker.Core.Domain.Tests;

public class TestScraperBuilder
{
    private Brand _brand;
    private Catalog _catalog;
    private Scraper _scraper;

    public TestScraperBuilder()
    {
        _brand = Brand.Create(Name.From("Test Brand"));
        _catalog = Catalog.Create(_brand.Id, Name.From("Test Brand"), new ProductSchema(new ProductIdPropertySchema(Name.From("Identifier")), new ProductNamePropertySchema(Name.From("Name")), new ProductPricePropertySchema(Name.From("Price")), new List<ProductPropertySchema>()));
        
        var parser = new ParserBuilder()
            .WithElementSelector("article")
            .AddProperty(PropertyName.Identifier.Value, "h1", ValueKind.Text)
            .AddProperty(PropertyName.DisplayName.Value, "h5", ValueKind.Text)
            .AddProperty("value", "div", ValueKind.Decimal)
            .Build();

        _scraper = Scraper.Create(_brand.Id, _catalog.Id, ScraperUri.From("https://example.com/page"), parser);
    }

    public TestScraperBuilder WithPagination(int maxPages)
    { 
        _scraper.ConfigurePagination(ParameterName.From("p"), MaxPages.From(maxPages));
        return this;
    }

    public (Brand brand, Catalog catalog, Scraper scrapper) Build()
    {
        return (_brand, _catalog, _scraper);
    }
}
