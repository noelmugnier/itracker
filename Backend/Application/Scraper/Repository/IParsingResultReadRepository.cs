namespace ITracker.Core.Application;

public interface IParsingResultReadRepository
{
	Task<Result<ParsingResultDto>> FindById(ParsingResultId id, CancellationToken token);
	Task<Result<IEnumerable<ParsingResultDto>>> List(ScraperId id, PageNumber pageNumber, PageSize pageSize, CancellationToken token);
}
