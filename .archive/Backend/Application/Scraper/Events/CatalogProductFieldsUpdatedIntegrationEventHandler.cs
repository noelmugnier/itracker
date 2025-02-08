namespace ITracker.Core.Application;

internal class CatalogProductFieldsUpdatedIntegrationEventHandler : IntegrationEventHandler<CatalogProductFieldsUpdatedIntegrationEvent>
{
	private readonly IUnitOfWork _uow;

	public CatalogProductFieldsUpdatedIntegrationEventHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public override async Task<Result> HandleEvent(CatalogProductFieldsUpdatedIntegrationEvent integrationEvent, CancellationToken token)
	{		
		var repository = _uow.Get<IScraperRepository>();
		var scrapersResult = await repository.GetAllForCatalog(CatalogId.From(integrationEvent.CatalogId), token);
		if(scrapersResult.IsFailed)
			return scrapersResult.ToResult();

		foreach(var scraper in scrapersResult.Value.ToList())
		{
			scraper.RequireConfigurationReview("Catalog product fields were updated");
			var updateResult = await repository.Update(scraper, token);
			if(updateResult.IsFailed)
				return updateResult;
		}

		return Result.Ok();
	}
}
