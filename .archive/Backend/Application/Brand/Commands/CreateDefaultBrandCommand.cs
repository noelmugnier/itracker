namespace ITracker.Core.Application;

public record CreateDefaultBrandCommand(string Name) : ICommand<Guid>;

public class CreateDefaultBrandCommandValidator : AbstractValidator<CreateDefaultBrandCommand>
{
	public CreateDefaultBrandCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCode.BrandNameIsRequired);
    }
}

internal class CreateDefaultBrandCommandHandler : ICommandHandler<CreateDefaultBrandCommand, Guid>
{
	private readonly IUnitOfWork _uow;

	public CreateDefaultBrandCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result<Guid>> Handle(CreateDefaultBrandCommand request, CancellationToken token)
	{
		var brandRepository = _uow.Get<IBrandRepository>();
		var brand = Brand.CreateDefault(Name.From(request.Name));
		
		var insertResult = await brandRepository.Insert(brand, token);
		if (insertResult.IsFailed)
			return insertResult;

		var commitResult = await _uow.Commit(token);
		if (commitResult.IsFailed)
			return commitResult;

		return Result.Ok(brand.Id.Value);
	}
}
