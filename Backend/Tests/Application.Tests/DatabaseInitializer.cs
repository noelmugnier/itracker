namespace ITracker.Core.Application.Tests;

public class TestDatabaseBuilder
{
	private readonly FakeUnitOfWork _uow;

	public TestDatabaseBuilder()
	{
		_uow = new FakeUnitOfWork();
	}

	public TestDatabaseBuilder AddBrand(out BrandId brandId, bool isDefault = false)
	{
		var brand = isDefault ? Brand.CreateDefault(Name.From("default brand name")) : Brand.Create(Name.From("brand name"));
		brandId = brand.Id;

		_uow.Get<IBrandRepository>().Insert(brand, CancellationToken.None).Wait();
		return this;
	}

	public TestDatabaseBuilder AddCatalog(BrandId brandId, out CatalogId catalogId, List<ProductPropertySchema>? fields = null)
	{
		var catalog = Catalog.Create(brandId, Name.From("Test Catalog"), new ProductSchema(new ProductIdPropertySchema(Name.From("Identifier")), new ProductNamePropertySchema(Name.From("Name")), new ProductPricePropertySchema(Name.From("Price")), fields != null && fields.Any() ? fields : new List<ProductPropertySchema>()));
		catalogId = catalog.Id;

		_uow.Get<ICatalogRepository>().Insert(catalog, CancellationToken.None).Wait();
		return this;
	}

	public TestDatabaseBuilder AddProduct(CatalogId catalogId, out ProductId productId, List<PropertyParsed>? fields = null)
	{
		var product = Product.Create(fields != null && fields.Any() ? fields : new List<PropertyParsed>
		{
			new TextParsed(PropertyName.Identifier, new Text(Guid.NewGuid().ToString("N"))),
			new TextParsed(PropertyName.DisplayName, new Text("super name")),
			new MoneyParsed(PropertyName.Price, new Money(10, "€")),
		}, catalogId, new List<ProductPropertySchema> {
			new ProductIdPropertySchema(Name.From("Identifier")),
			new ProductNamePropertySchema(Name.From("Name")),
			new ProductPricePropertySchema(Name.From("Price")),  });

		productId = product.Id;

		_uow.Get<IProductRepository>().Insert(product, CancellationToken.None).Wait();
		return this;
	}

	public TestDatabaseBuilder AddScraper(BrandId brandId, CatalogId catalogId, out ScraperId scraperId, ScraperStatus status = ScraperStatus.Active)
	{
		var scraper = Scraper.Create(brandId, catalogId, ScraperUri.From("https://example.com/page"), _selector, Name.From("Test Scraper"));
		scraperId = scraper.Id;

		InitScraperStatus(status, scraper);

		_uow.Get<IScraperRepository>().Insert(scraper, CancellationToken.None).Wait();
		return this;
	}

	public TestDatabaseBuilder AddScheduledScraper(BrandId brandId, CatalogId catalogId, string cron, out ScraperId scraperId, ScraperStatus status = ScraperStatus.Active)
	{
		var scraper = Scraper.Create(brandId, catalogId, ScraperUri.From("https://example.com/page"), _selector, Name.From("Test Scraper"));
		scraperId = scraper.Id;

		scraper.ConfigureScheduling(Cron.From(cron));

		InitScraperStatus(status, scraper);

		_uow.Get<IScraperRepository>().Insert(scraper, CancellationToken.None).Wait();
		return this;
	}

	public TestDatabaseBuilder AddPaginatedScraper(BrandId brandId, CatalogId catalogId, int maxPages, out ScraperId scraperId, ScraperStatus status = ScraperStatus.Active)
	{
		var scraper = Scraper.Create(brandId, catalogId, ScraperUri.From("https://example.com/page"), _selector, Name.From("Test Scraper"));
		scraperId = scraper.Id;

		scraper.ConfigurePagination(ParameterName.From("page"), MaxPages.From(maxPages));

		InitScraperStatus(status, scraper);

		_uow.Get<IScraperRepository>().Insert(scraper, CancellationToken.None).Wait();
		return this;
	}

	public FakeUnitOfWork Build()
	{
		_uow.Commit(CancellationToken.None).Wait();
		return _uow;
	}

	private Parser _selector = new ParserBuilder()
		.WithElementSelector("root")
		.Build();

	private void InitScraperStatus(ScraperStatus status, Scraper scraper)
	{
		if (status == ScraperStatus.Disabled)
			scraper.Disable();

		if (status == ScraperStatus.ReviewRequired)
			scraper.RequireConfigurationReview("Review required");
	}

}
