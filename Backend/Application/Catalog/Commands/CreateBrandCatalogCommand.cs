namespace ITracker.Core.Application;

public record CreateBrandCatalogCommand(Guid BrandId, string Name, ProductSchemaDto ProductDefinition) : ICommand<Guid>;

public class CreateBrandCatalogCommandValidator : AbstractValidator<CreateBrandCatalogCommand>
{
	public CreateBrandCatalogCommandValidator()
	{
		RuleFor(x => x.BrandId).NotEmpty().WithErrorCode(ErrorCode.BrandIdIsRequired);
		RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCode.CatalogNameIsRequired);
		RuleFor(x => x.ProductDefinition).NotNull().WithErrorCode(ErrorCode.ProductDefinitionIsRequired);
		RuleFor(x => x.ProductDefinition).SetValidator(new ProductSchemaDtoValidator());
    }
}

internal class CreateBrandCatalogCommandHandler : ICommandHandler<CreateBrandCatalogCommand, Guid>
{
	private readonly IUnitOfWork _uow;

	public CreateBrandCatalogCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result<Guid>> Handle(CreateBrandCatalogCommand request, CancellationToken token)
	{
		var repository = _uow.Get<ICatalogRepository>();

		var productSchema = new ProductSchemaBuilder()
			.WithIdentifierProperty(request.ProductDefinition.IdentifierDisplayName)
			.WithNameProperty(request.ProductDefinition.NameDisplayName)
      .WithPriceProperty(request.ProductDefinition.PriceDisplayName);

		request.ProductDefinition.Fields?.ToList().ForEach(f => productSchema.AddField(f.PropertyName, f.DisplayName, f.ValueType, f.Required, f.Tracked));

		var catalog = Catalog.Create(BrandId.From(request.BrandId), Name.From(request.Name), productSchema.Build());

		var insertResult = await repository.Insert(catalog, token);
		if (insertResult.IsFailed)
			return insertResult;

		var commitResult = await _uow.Commit(token);
		if (commitResult.IsFailed)
			return commitResult;
		
		return catalog.Id.Value;
	}
}
