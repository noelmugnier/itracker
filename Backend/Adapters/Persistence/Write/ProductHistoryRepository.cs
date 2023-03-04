using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace ITracker.Adapters.Persistence;

public class ProductHistoryRepository : IProductHistoryRepository
{
	private readonly DbSet<ProductHistory> _set;

	public ProductHistoryRepository(DbContext context)
	{
		_set = context.Set<ProductHistory>();
	}

	public async Task<OneOf<ProductHistory, NotFoundError, UnexpectedError>> Get(Guid id, CancellationToken token)
	{
		try
		{
			var result = await _set.SingleOrDefaultAsync(p => p.Id == id, token);
			return result != null ? result : NotFoundError.From<Guid>(id);
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result<IEnumerable<ProductHistory>>> GetProductHistory(CatalogId catalogId, ProductId productId, CancellationToken token)
	{
		try
		{
			var results = await _set
				.Where(p => p.CatalogId == catalogId && p.ProductId == productId)
				.ToListAsync(token);

			return results;
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result> Insert(ProductHistory productHistory, CancellationToken token)
	{
		try
		{
			await _set.AddAsync(productHistory, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}
	}
}
