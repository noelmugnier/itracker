using ITracker.Core.Domain;
using FluentResults;
using OneOf;
using Microsoft.EntityFrameworkCore;

namespace ITracker.Adapters.Persistence;

public class BrandRepository : IBrandRepository
{
	private readonly DbSet<Brand> _set;

	public BrandRepository(DbContext context)
	{
		_set = context.Set<Brand>();
	}

	public async Task<Result> Insert(Brand brand, CancellationToken token)
	{
		try
		{
			await _set.AddAsync(brand, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(new UnexpectedError(e));
		}
	}

	public Task<Result> Update(Brand brand, CancellationToken token)
	{
		return Task.FromResult(Result.Ok());
	}

	public async Task<OneOf<Brand, NotFoundError, UnexpectedError>> Get(BrandId id, CancellationToken token)
	{
		try
		{
			var result = await _set.SingleOrDefaultAsync(p => p.Id == id, token);
			return result != null ? result : NotFoundError.From<BrandId>(id);
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public Task<Result> Delete(Brand brand, CancellationToken token)
	{
		try
		{
			_set.Remove(brand);
			return Task.FromResult(Result.Ok());
		}
		catch (Exception exc)
		{
			return Task.FromResult(Result.Fail(new UnexpectedError(exc)));
		}
	}
}
