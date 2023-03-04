namespace ITracker.Core.Application.Tests;

public class FakeScraperFieldsRetriever : IScraperFieldsRetriever
{
	private readonly IUnitOfWork _uow;

	public FakeScraperFieldsRetriever(IUnitOfWork uow)
    {
        _uow = uow;
    }

	public async Task<Result<IEnumerable<PropertyFieldSchema>>> RetrieveCatalogScraperFields(CatalogId catalogId, CancellationToken token)
	{
        var repository = _uow.Get<ICatalogRepository>();
        var catalogResult = await repository.Get(catalogId, token);
        if (!catalogResult.IsT0)
            return Result.Fail<IEnumerable<PropertyFieldSchema>>("catalog not found");

        return Result.Ok(catalogResult.AsT0.GetAllSchemaFields());
	}
}
