using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ITracker.Core.Application;

namespace ITracker.Adapters.Persistence;

public class ProductReadRepository : IProductReadRepository
{
	private readonly DbSet<Product> _set;

	public ProductReadRepository(AppDbContext context)
	{
		_set = context.Set<Product>();
	}

	public async Task<Result<ProductDto>> FindById(CatalogId catalogId, ProductId productId, CancellationToken token)
	{
		try
		{
			var result = await _set
				.AsNoTracking()
				.SingleOrDefaultAsync(t => t.Id == productId && t.CatalogId == catalogId, token);

			return result != null 
				? new ProductDto(result)
				: NotFoundError.From<ProductId>(productId);
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result<IEnumerable<ProductDto>>> List(CatalogId catalogId, PageNumber pageNumber, PageSize pageSize, CancellationToken token)
	{
		try
		{
			var results = await _set
				.AsNoTracking()
				.Where(c => c.CatalogId == catalogId)
				.OrderByDescending(t => t.CreatedOn)
				.Skip((pageNumber.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToListAsync(token);

			return results.Select(t => new ProductDto(t)).ToList();
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}
}
