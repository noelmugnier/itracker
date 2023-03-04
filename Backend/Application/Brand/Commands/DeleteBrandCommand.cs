namespace ITracker.Core.Application;

public record DeleteBrandCommand(Guid BrandId) : ICommand;

public class DeleteBrandCommandValidator : AbstractValidator<DeleteBrandCommand>
{
	public DeleteBrandCommandValidator()
	{
		RuleFor(x => x.BrandId).NotEmpty().WithErrorCode(ErrorCode.BrandIdIsInvalid);
	}
}

internal class DeleteBrandCommandHandler : ICommandHandler<DeleteBrandCommand>
{
	private readonly IUnitOfWork _uow;

	public DeleteBrandCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result> Handle(DeleteBrandCommand request, CancellationToken token)
	{
		var repository = _uow.Get<IBrandRepository>();
		var getResult = await repository.Get(BrandId.From(request.BrandId), token);

		return await getResult.Match<Task<Result>>(
			async brand => { 
				var deleteResult = await repository.Delete(brand, token);
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

