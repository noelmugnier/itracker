using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ITracker.Core.Application;

namespace ITracker.Adapters.Persistence;

public class ParsingResultReadRepository : IParsingResultReadRepository
{
	private readonly DbSet<ParsingResult> _set;

	public ParsingResultReadRepository(AppDbContext context)
	{
		_set = context.Set<ParsingResult>();
	}

	public async Task<Result<ParsingResultDto>> FindById(ParsingResultId id, CancellationToken token)
	{
		try
		{
			var result = await _set
				.AsNoTracking()
				.SingleOrDefaultAsync(t => t.Id == id, token);
				
			return result != null 
				? new ParsingResultDto(result)
				: NotFoundError.From<ParsingResultId>(id);
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result<IEnumerable<ParsingResultDto>>> List(ScraperId scraperId, PageNumber pageNumber, PageSize pageSize, CancellationToken token)
	{
		try
		{
			var results = await _set
				.AsNoTracking()
				.Where(t => t.ScraperId == scraperId)
				.OrderByDescending(t => t.CreatedOn)
				.Skip((pageNumber.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToListAsync(token);

			return results.Select(t => new ParsingResultDto(t)).ToList();
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}
}
