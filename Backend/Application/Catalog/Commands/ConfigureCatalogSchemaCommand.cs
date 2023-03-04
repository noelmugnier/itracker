namespace ITracker.Core.Application;

public record ConfigureCatalogSchemaCommand(Guid CatalogId, ProductSchemaDto ProductDefinition) : ICommand;

public class ConfigureCatalogSchemaCommandValidator : AbstractValidator<ConfigureCatalogSchemaCommand>
{
	public ConfigureCatalogSchemaCommandValidator()
	{
		RuleFor(x => x.CatalogId).NotEmpty().WithErrorCode(ErrorCode.CatalogIdIsRequired);
		RuleFor(x => x.ProductDefinition).NotNull().WithErrorCode(ErrorCode.ProductDefinitionIsRequired);
		RuleFor(x => x.ProductDefinition).SetValidator(new ProductSchemaDtoValidator());
    }
}

internal class ConfigureCatalogSchemaCommandHandler : ICommandHandler<ConfigureCatalogSchemaCommand>
{
	private readonly IUnitOfWork _uow;

	public ConfigureCatalogSchemaCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(ConfigureCatalogSchemaCommand request, CancellationToken token)
	{
		var repository = _uow.Get<ICatalogRepository>();

		var productSchema = new ProductSchemaBuilder()
			.WithIdentifierProperty(request.ProductDefinition.IdentifierDisplayName)
			.WithNameProperty(request.ProductDefinition.NameDisplayName)
      .WithPriceProperty(request.ProductDefinition.PriceDisplayName);

		request.ProductDefinition.Fields?.ToList().ForEach(f => productSchema.AddField(f.PropertyName, f.DisplayName, f.ValueType, f.Required, f.Tracked));

		var catalogResult = await repository.Get(CatalogId.From(request.CatalogId), token);
		return await catalogResult.Match<Task<Result>>(
			async catalog =>
			{
				catalog.ConfigureSchema(productSchema.Build());

				var updateResult = await repository.Update(catalog, token);
				if(updateResult.IsFailed)
					return updateResult;

				return await _uow.Commit(token);
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			error => Task.FromResult(Result.Fail(error))
		);
	}
}
