namespace ITracker.Core.Application;

public record GetBrandQuery(Guid BrandId) : IQuery<BrandDto>;

public class GetBrandQueryValidator : AbstractValidator<GetBrandQuery>
{
	public GetBrandQueryValidator()
	{
		RuleFor(x => x.BrandId).NotEmpty().WithErrorCode(ErrorCode.BrandIdIsInvalid);
    }
}

internal class GetBrandQueryHandler : IQueryHandler<GetBrandQuery, BrandDto>
{
	private readonly IBrandReadRepository _repository;

	public GetBrandQueryHandler(IBrandReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<BrandDto>> Handle(GetBrandQuery request, CancellationToken token)
	{
		return _repository.FindById(BrandId.From(request.BrandId), token);
	}
}
