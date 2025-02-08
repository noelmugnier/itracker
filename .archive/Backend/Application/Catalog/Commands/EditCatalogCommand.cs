namespace ITracker.Core.Application;

public record EditCatalogCommand(Guid CatalogId, string Name) : ICommand;

public class EditCatalogCommandValidator : AbstractValidator<EditCatalogCommand>
{
	public EditCatalogCommandValidator()
	{
		RuleFor(x => x.CatalogId).NotEmpty().WithErrorCode(ErrorCode.CatalogIdIsRequired);
		RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCode.CatalogNameIsRequired);
    }
}

internal class EditCatalogCommandHandler : ICommandHandler<EditCatalogCommand>
{
	private readonly IUnitOfWork _uow;

	public EditCatalogCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(EditCatalogCommand request, CancellationToken token)
	{
		var repository = _uow.Get<ICatalogRepository>();

		var catalogResult = await repository.Get(CatalogId.From(request.CatalogId), token);
		return await catalogResult.Match<Task<Result>>(
			async catalog =>
			{
				catalog.Name = Name.From(request.Name);

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