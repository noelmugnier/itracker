namespace ITracker.Core.Domain;

public record Pagination
{
	private Pagination(){}

	internal Pagination(ParameterName pageNumberParameterName, MaxPages maxPages)
	{
		PageNumberParameterName = pageNumberParameterName;
		MaxPages = maxPages;
	}

	public ParameterName PageNumberParameterName { get; }
	public MaxPages MaxPages { get; }
	public PageSize? PageSize { get; init; }
	public ParameterName? PageSizeParameterName { get; init; }
}

