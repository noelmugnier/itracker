using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ITracker.Core.Application;

namespace ITracker.Adapters.Persistence;

public class ProductHistoryReadRepository : IProductHistoryReadRepository
{
	private readonly DbSet<ProductHistory> _set;

	public ProductHistoryReadRepository(AppDbContext context)
	{
		_set = context.Set<ProductHistory>();
	}

	public async Task<Result<IEnumerable<ProductHistoryDto>>> List(CatalogId catalogId, ProductId productId, PageNumber pageNumber, PageSize pageSize, CancellationToken token)
	{
		try
		{
			var results = await _set
				.AsNoTracking()
				.Where(c => c.CatalogId == catalogId && c.ProductId == productId)
				.OrderByDescending(t => t.CreatedOn)
				.Skip((pageNumber.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToListAsync(token);

			return results.Select(t => new ProductHistoryDto(t)).ToList();
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}
}
