namespace ITracker.Core.Application;

public record CreateBrandCommand(string Name) : ICommand<Guid>;

public class CreateBrandCommandValidator : AbstractValidator<CreateBrandCommand>
{
	public CreateBrandCommandValidator()
	{
		RuleFor(x => x.Name).NotEmpty().WithErrorCode(ErrorCode.BrandNameIsRequired);
    }
}

internal class CreateBrandCommandHandler : ICommandHandler<CreateBrandCommand, Guid>
{
	private readonly IUnitOfWork _uow;

	public CreateBrandCommandHandler(IUnitOfWork uow)
	{
		_uow = uow;
	}

	public async Task<Result<Guid>> Handle(CreateBrandCommand request, CancellationToken token)
	{
		var brandRepository = _uow.Get<IBrandRepository>();
		var brand = Brand.Create(Name.From(request.Name));
		
		var insertResult = await brandRepository.Insert(brand, token);
		if (insertResult.IsFailed)
			return insertResult;

		var commitResult = await _uow.Commit(token);
		if (commitResult.IsFailed)
			return commitResult;

		return Result.Ok(brand.Id.Value);
	}
}