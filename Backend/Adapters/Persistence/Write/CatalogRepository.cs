using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace ITracker.Adapters.Persistence;

public class CatalogRepository : ICatalogRepository
{
	private readonly DbSet<Catalog> _set;

	public CatalogRepository(DbContext context)
	{
		_set = context.Set<Catalog>();
	}

	public async Task<OneOf<Catalog, NotFoundError, UnexpectedError>> Get(CatalogId id, CancellationToken token)
	{
		try
		{
			var result = await _set.SingleOrDefaultAsync(p => p.Id == id, token);
			return result != null ? result : NotFoundError.From<CatalogId>(id);
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result> Insert(Catalog catalog, CancellationToken token)
	{
		try
		{
			await _set.AddAsync(catalog, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}
	}

	public Task<Result> Update(Catalog catalog, CancellationToken token)
	{
		return Task.FromResult(Result.Ok());
	}

	public Task<Result> Delete(Catalog catalog, CancellationToken token)
	{
		try
		{
			_set.Remove(catalog);
			return Task.FromResult(Result.Ok());
		}
		catch (Exception e)
		{
			return Task.FromResult(Result.Fail(e.Message));
		}
		
	}
}
