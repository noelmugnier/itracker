using ITracker.Core.Domain;
using FluentResults;
using OneOf;
using Microsoft.EntityFrameworkCore;

namespace ITracker.Adapters.Persistence;

public class ScraperRepository : IScraperRepository
{
	private readonly DbSet<Scraper> _set;

	public ScraperRepository(DbContext context)
	{
		_set = context.Set<Scraper>();
	}

	public async Task<Result> Insert(Scraper scraper, CancellationToken token)
	{
		try
		{
			await _set.AddAsync(scraper, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(new UnexpectedError(e));
		}
	}

	public Task<Result> Update(Scraper scraper, CancellationToken token)
	{
		return Task.FromResult(Result.Ok());
	}

	public async Task<OneOf<Scraper, NotFoundError, UnexpectedError>> Get(ScraperId id, CancellationToken token)
	{
		try
		{
			var result = await _set.SingleOrDefaultAsync(p => p.Id == id, token);
			return result != null ? result : NotFoundError.From<ScraperId>(id);
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public Task<Result> Delete(Scraper scraper, CancellationToken token)
	{
		try
		{
			_set.Remove(scraper);
			return Task.FromResult(Result.Ok());
		}
		catch (Exception exc)
		{
			return Task.FromResult(Result.Fail(new UnexpectedError(exc)));
		}
	}

	public async Task<Result<IEnumerable<Scraper>>> GetAllForCatalog(CatalogId catalogId, CancellationToken token)
	{
		try
		{
			var results = await _set
				.Where(p => p.CatalogId == catalogId)
				.ToListAsync(token);

			return results;
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result<IEnumerable<Scraper>>> GetAllScheduledScrapers(CancellationToken token)
	{
		try
		{
			var results = await _set
				.Where(p => p.Scheduling != null)
				.ToListAsync(token);

			return results;
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}
}
