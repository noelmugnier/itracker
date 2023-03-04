namespace ITracker.Core.Domain;

public interface IScraperFieldsRetriever 
{
	public Task<Result<IEnumerable<PropertyFieldSchema>>> RetrieveCatalogScraperFields(CatalogId catalogId, CancellationToken token);
}