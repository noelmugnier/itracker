namespace ITracker.Core.Domain;

public interface IParsingResultRepository : IRepository<ParsingResult, ParsingResultId>
{
	Task<Result> Insert(ParsingResult parsingResult, CancellationToken token);
}
