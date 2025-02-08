namespace ITracker.Core.Application;

public record DeleteCatalogCommand(Guid CatalogId) : ICommand;

public class DeleteCatalogCommandValidator : AbstractValidator<DeleteCatalogCommand>
{
	public DeleteCatalogCommandValidator()
	{
		RuleFor(x => x.CatalogId).NotEmpty().WithErrorCode(ErrorCode.CatalogIdIsRequired);
    }
}

internal class DeleteCatalogCommandHandler : ICommandHandler<DeleteCatalogCommand>
{
	private readonly IUnitOfWork _uow;

	public DeleteCatalogCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(DeleteCatalogCommand request, CancellationToken token)
	{
		var repository = _uow.Get<ICatalogRepository>();
		var getResult = await repository.Get(CatalogId.From(request.CatalogId), token);
		
		return await getResult.Match<Task<Result>>(
			async catalog => { 
				var deleteResult = await repository.Delete(catalog, token);
				if (deleteResult.IsFailed)
					return deleteResult;

				var commitResult = await _uow.Commit(token);
				if (commitResult.IsFailed)
					return commitResult;

				return Result.Ok();
			},
			notFound => Task.FromResult(Result.Fail(notFound)),
			unexpected => Task.FromResult(Result.Fail(unexpected)));	
	}
}