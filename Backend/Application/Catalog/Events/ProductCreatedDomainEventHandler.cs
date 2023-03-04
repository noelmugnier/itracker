namespace ITracker.Core.Application;

internal class ProductCreatedDomainEventHandler : DomainEventHandler<ProductCreatedDomainEvent>
{
	private readonly IUnitOfWork _uow;

	public ProductCreatedDomainEventHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public override async Task<Result> HandleEvent(ProductCreatedDomainEvent notification, CancellationToken token)
	{
		var repository = _uow.Get<IProductHistoryRepository>();

		var productHistory = ProductHistory.Create(notification.CatalogId, notification.ProductId, notification.Price, notification.OccuredOn, notification.Fields);
		var productResult = await repository.Insert(productHistory, token);
		if (productResult.IsFailed)
			return productResult;

		return await _uow.Commit(token);
	}
}