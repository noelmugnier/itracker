namespace ITracker.Core.Application;

public record CreateCatalogsMappingCommand(Guid BrandId, CatalogsMappingDto CatalogsMapping) : ICommand;

internal class CreateCatalogsMappingCommandHandler : ICommandHandler<CreateCatalogsMappingCommand>
{
	private readonly IUnitOfWork _uow;

	public CreateCatalogsMappingCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(CreateCatalogsMappingCommand request, CancellationToken token)
	{
		var repository = _uow.Get<ICatalogProductMappingRepository>();	
    var brandRepository = _uow.Get<IBrandRepository>();

    var getBrand = await brandRepository.Get(BrandId.From(request.BrandId), token);
    return await getBrand.Match<Task<Result>>(async (brand) => 
      {
        if(!brand.IsDefault)
          return Result.Fail(new DomainError(ErrorCode.SourceCatalogMustBeInDefaultBrand));

        foreach (var product in request.CatalogsMapping.Products)
        {
          var catalogProductMapping = CatalogProductMapping.Create(CatalogId.From(request.CatalogsMapping.SourceCatalogId), ProductId.From(product.SourceProductId), CatalogId.From(request.CatalogsMapping.TargetCatalogId), ProductId.From(product.TargetProductId));
          var insertResult = await repository.Insert(catalogProductMapping, token);
          if (insertResult.IsFailed)
            return insertResult;
        }

        var commitResult = await _uow.Commit(token);
        if (commitResult.IsFailed)
          return commitResult;

        return Result.Ok();
      },
      notFound => Task.FromResult(Result.Fail(notFound)),
      error => Task.FromResult(Result.Fail(error))
    );
	}
}