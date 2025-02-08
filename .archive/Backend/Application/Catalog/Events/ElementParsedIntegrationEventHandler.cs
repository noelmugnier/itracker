namespace ITracker.Core.Application;

public record ElementParsedIntegrationEvent(Guid CatalogId, IEnumerable<PropertyParsed> ParsedProperties) : IntegrationEvent;

internal class ElementParsedIntegrationEventHandler : IntegrationEventHandler<ElementParsedIntegrationEvent>
{
	private readonly IUnitOfWork _uow;

	public ElementParsedIntegrationEventHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public override async Task<Result> HandleEvent(ElementParsedIntegrationEvent domainEvent, CancellationToken token)
	{
		var catalogRepository = _uow.Get<ICatalogRepository>();
		var catalogId = CatalogId.From(domainEvent.CatalogId);

		var catalogResult = await catalogRepository.Get(catalogId, token);
		if (!catalogResult.IsT0 && catalogResult.IsT1)
			return Result.Fail(catalogResult.AsT1);

		if (!catalogResult.IsT0 && catalogResult.IsT2)
			return Result.Fail(catalogResult.AsT2);

		var catalog = catalogResult.AsT0;
		var productId = Product.GetIdentifierFromFields(domainEvent.ParsedProperties);

		var productRepository = _uow.Get<IProductRepository>();
		var getProductResult = await productRepository.Get(productId, token);

		return await getProductResult.Match<Task<Result>>(
			async existingProduct =>
			{
				existingProduct.UpdateWithLastParsedElementData(domainEvent.ParsedProperties, catalog.GetProductProperties());

				var updateResult = await productRepository.Update(existingProduct, token);
				if (updateResult.IsFailed)
					return updateResult;

				return await _uow.Commit(token);
			},
			async notFound =>
			{
				var product = Product.Create(domainEvent.ParsedProperties, catalog.Id, catalog.GetProductProperties());
				var insertResult = await productRepository.Insert(product, token);
				if (insertResult.IsFailed)
					return insertResult;

				return await _uow.Commit(token);
			},
			error => Task.FromResult(Result.Fail(error)));
	}
}
