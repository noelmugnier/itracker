namespace ITracker.Core.Application;

public record GetParsingResultQuery(Guid ParsingResultId) : IQuery<ParsingResultDto>;

public class GetParsingResultQueryValidator : AbstractValidator<GetParsingResultQuery>
{
	public GetParsingResultQueryValidator()
	{
		RuleFor(x => x.ParsingResultId).NotEmpty().WithErrorCode(ErrorCode.ParsingResultIdIsInvalid);
    }
}

internal class GetParsingResultQueryHandler : IQueryHandler<GetParsingResultQuery, ParsingResultDto>
{
	private readonly IParsingResultReadRepository _repository;

	public GetParsingResultQueryHandler(IParsingResultReadRepository repository)
	{
		_repository = repository;
	}

	public Task<Result<ParsingResultDto>> Handle(GetParsingResultQuery request, CancellationToken token)
	{
		return _repository.FindById(ParsingResultId.From(request.ParsingResultId), token);
	}
}
