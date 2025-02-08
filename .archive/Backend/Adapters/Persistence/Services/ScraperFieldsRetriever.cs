using FluentResults;
using ITracker.Core.Application;
using ITracker.Core.Domain;

namespace ITracker.Adapters.Persistence;

public class ScraperFieldsRetriever : IScraperFieldsRetriever
{
	private readonly IUnitOfWork _uow;

	public ScraperFieldsRetriever(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result<IEnumerable<PropertyFieldSchema>>> RetrieveCatalogScraperFields(CatalogId catalogId, CancellationToken token)
	{
		var repository = _uow.Get<ICatalogRepository>();
		var catalogResult = await repository.Get(catalogId, token);

		return catalogResult.Match<Result<IEnumerable<PropertyFieldSchema>>>(
			catalog => Result.Ok(catalog.GetAllSchemaFields()),
			notFound => Result.Fail(notFound),
			error => Result.Fail(error)
		);
	}
}
