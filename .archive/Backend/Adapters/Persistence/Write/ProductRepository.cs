using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using OneOf;

namespace ITracker.Adapters.Persistence;

public class ProductRepository : IProductRepository
{
	private readonly DbSet<Product> _set;

	public ProductRepository(DbContext context)
	{
		_set = context.Set<Product>();
	}

	public async Task<OneOf<Product, NotFoundError, UnexpectedError>> Get(ProductId id, CancellationToken token)
	{
		try
		{
			var result = await _set.SingleOrDefaultAsync(p => p.Id == id, token);
			return result != null ? result : NotFoundError.From<ProductId>(id);
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result> Insert(Product product, CancellationToken token)
	{
		try
		{
			await _set.AddAsync(product, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}
	}

	public Task<Result> Update(Product product, CancellationToken token)
	{
		return Task.FromResult(Result.Ok());
	}
}
