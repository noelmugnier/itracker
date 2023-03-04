namespace ITracker.Core.Application;

public record ScraperDto
{
	public ScraperDto(Guid id, string name, Uri uri, ScraperStatus status, ParserDto parser, SchedulingDto? scheduling, PaginationDto? pagination)
	{
		Id = id;
		Name = name;
		Uri = uri;
		Status = status;
		Parser = parser;
		Scheduling = scheduling;
		Pagination = pagination;		
	}

	public ScraperDto(Scraper scraper)
	:this(scraper.Id.Value, scraper.Name.Value, scraper.Uri.Value, scraper.Status, new ParserDto(scraper.Parser), scraper.Scheduling is not null ? new SchedulingDto(scraper.Scheduling) : null, scraper.Pagination is not null ? new PaginationDto(scraper.Pagination) : null)
	{		
	}

	public Guid Id { get; }
	public string Name { get; }
	public Uri Uri { get; }
	public ScraperStatus Status { get; }
	public ParserDto Parser { get; }
	public SchedulingDto? Scheduling { get; }
	public PaginationDto? Pagination { get; }
}
  