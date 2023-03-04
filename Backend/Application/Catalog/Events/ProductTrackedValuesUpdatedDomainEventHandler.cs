namespace ITracker.Core.Application;

internal class ProductTrackedValuesUpdatedDomainEventHandler : DomainEventHandler<ProductTrackedValuesUpdatedDomainEvent>
{
	private readonly IUnitOfWork _uow;

	public ProductTrackedValuesUpdatedDomainEventHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public override async Task<Result> HandleEvent(ProductTrackedValuesUpdatedDomainEvent notification, CancellationToken token)
	{
		var repository = _uow.Get<IProductHistoryRepository>();

		var productHistory = ProductHistory.Create(notification.CatalogId, notification.ProductId, notification.Price, notification.OccuredOn, notification.ModifiedFields);
		var productResult = await repository.Insert(productHistory, token);
		if (productResult.IsFailed)
			return productResult;

		return await _uow.Commit(token);
	}
}