using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ITracker.Core.Application;

namespace ITracker.Adapters.Persistence;

public class CatalogReadRepository : ICatalogReadRepository
{
	private readonly DbSet<Catalog> _set;

	public CatalogReadRepository(AppDbContext context)
	{
		_set = context.Set<Catalog>();
	}

	public async Task<Result<CatalogDto>> FindById(CatalogId id, CancellationToken token)
	{
		try
		{
			var result = await _set
				.AsNoTracking()
				.SingleOrDefaultAsync(t => t.Id == id, token);

			return result != null 
				? new CatalogDto(result)
				: NotFoundError.From<CatalogId>(id);
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result<IEnumerable<CatalogDto>>> List(BrandId brandId, PageNumber pageNumber, PageSize pageSize, CancellationToken token)
	{
		try
		{
			var results = await _set
				.AsNoTracking()
				.Where(c => c.BrandId == brandId)
				.OrderByDescending(t => t.CreatedOn)
				.Skip((pageNumber.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToListAsync(token);

			return results.Select(t => new CatalogDto(t)).ToList();
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}
}
