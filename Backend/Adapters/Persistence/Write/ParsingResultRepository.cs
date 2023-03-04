using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace ITracker.Adapters.Persistence;

public class ParsingResultRepository : IParsingResultRepository
{
	private readonly DbSet<ParsingResult> _set;

	public ParsingResultRepository(DbContext context)
	{
		_set = context.Set<ParsingResult>();
	}

	public async Task<OneOf<ParsingResult, NotFoundError, UnexpectedError>> Get(ParsingResultId id, CancellationToken token)
	{
		try
		{
			var result = await _set.SingleOrDefaultAsync(p => p.Id == id, token);
			return result != null ? result : NotFoundError.From<ParsingResultId>(id);
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result> Insert(ParsingResult parsingResult, CancellationToken token)
	{
		try
		{
			await _set.AddAsync(parsingResult, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}
	}
}
