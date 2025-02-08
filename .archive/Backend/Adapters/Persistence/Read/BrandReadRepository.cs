using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ITracker.Core.Application;

namespace ITracker.Adapters.Persistence;

public class BrandReadRepository : IBrandReadRepository
{
	private readonly DbSet<Brand> _set;

	public BrandReadRepository(AppDbContext context)
	{
		_set = context.Set<Brand>();
	}

	public async Task<Result<BrandDto>> FindById(BrandId id, CancellationToken token)
	{
		try
		{
			var result = await _set
				.AsNoTracking()
				.SingleOrDefaultAsync(t => t.Id == id, token);

			return result != null 
				? new BrandDto(result)
				: NotFoundError.From<BrandId>(id);
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result<IEnumerable<BrandDto>>> List(PageNumber pageNumber, PageSize pageSize, CancellationToken token)
	{
		try
		{
			var results = await _set
				.AsNoTracking()
				.OrderByDescending(t => t.CreatedOn)
				.Skip((pageNumber.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToListAsync(token);

			return results.Select(t => new BrandDto(t)).ToList();
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}
}
