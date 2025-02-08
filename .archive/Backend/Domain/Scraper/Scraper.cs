namespace ITracker.Core.Domain;

public class Scraper : Entity<ScraperId>
{
	private Scraper()
		: base(ScraperId.New())
	{ }

	private Scraper(BrandId brandId, CatalogId catalogId, ScraperUri uri, Parser parser, Name? name = null)
		: base(ScraperId.New())
	{
		Status = ScraperStatus.Active;
		BrandId = brandId;
		CatalogId = catalogId;
		Name = name ?? Name.From(uri.Value.AbsolutePath.Replace('-', ' ').Replace('_', ' ').Trim('/').Replace("/", " - "));
		Uri = uri;
		Parser = parser;
		CreatedOn = DateTimeOffset.UtcNow;
		UpdatedOn = DateTimeOffset.UtcNow;
	}

	public static Scraper Create(BrandId brandId, CatalogId catalogId, ScraperUri uri, Parser parser, Name? name = null)
	{
		var scraper = new Scraper(brandId, catalogId, uri, parser, name);
		scraper.AddDomainEvent(new ScraperCreatedDomainEvent(scraper.Id));

		return scraper;
	}

	public Name Name { get; set; }
	public ScraperUri Uri { get; set; }
	public bool PaginationEnabled => Pagination != null;
	public bool SchedulingEnabled => Scheduling != null;
	public DateTimeOffset CreatedOn { get; }
	public ScraperStatus Status { get; private set; }
	public DateTimeOffset UpdatedOn { get; private set; }
	public Parser Parser { get; private set; }
	public Pagination? Pagination { get; private set; }
	public Scheduling? Scheduling { get; private set; }
	public CatalogId CatalogId { get; }
	public BrandId BrandId { get; }
	public string? Information { get; private set; }
	public bool CanBeScheduled => SchedulingEnabled && Status == ScraperStatus.Active;

	public void ConfigurePagination(ParameterName pageNumberParameterName, MaxPages maxPages, PageSize? pageSize = null, ParameterName? pageSizeParameterName = null)
	{
		Pagination = new Pagination(pageNumberParameterName, maxPages)
		{
			PageSize = pageSize,
			PageSizeParameterName = pageSizeParameterName
		};

		UpdatedOn = DateTimeOffset.UtcNow;
	}

	public void DisablePagination()
	{
		Pagination = null;
		UpdatedOn = DateTimeOffset.UtcNow;
	}

	public void ConfigureScheduling(Cron cron)
	{
		Scheduling = new Scheduling(cron);
		UpdatedOn = DateTimeOffset.UtcNow;

		AddDomainEvent(new ScraperScheduledDomainEvent(Id, cron));
	}

	public void DisableScheduling()
	{
		Scheduling = null;
		UpdatedOn = DateTimeOffset.UtcNow;

		AddDomainEvent(new ScraperUnscheduledDomainEvent(Id));
	}

	public void ConfigureParser(Parser selector)
	{
		Parser = selector;
		UpdatedOn = DateTimeOffset.UtcNow;
	}

	public void RequireConfigurationReview(string message)
	{
		Status = ScraperStatus.ReviewRequired;
		Information = message;
		UpdatedOn = DateTimeOffset.UtcNow;

		AddDomainEvent(new ScraperConfigurationReviewRequiredDomainEvent(Id));
	}

	public void Disable(string? message = null)
	{
		Status = ScraperStatus.Disabled;
		Information = message;
		UpdatedOn = DateTimeOffset.UtcNow;

		AddDomainEvent(new ScraperDisabledDomainEvent(Id));
	}

	public void Activate()
	{
		Status = ScraperStatus.Active;
		Information = null;
		UpdatedOn = DateTimeOffset.UtcNow;

		AddDomainEvent(new ScraperActivatedDomainEvent(Id));
	}
}
