using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ITracker.Core.Application;

namespace ITracker.Adapters.Persistence;

public class ScraperReadRepository : IScraperReadRepository
{
	private readonly DbSet<Scraper> _scrapers;
	private readonly DbSet<Catalog> _catalogs;

	public ScraperReadRepository(AppDbContext context)
	{
		_scrapers = context.Set<Scraper>();
		_catalogs = context.Set<Catalog>();
	}

	public async Task<Result<ScraperDto>> FindById(ScraperId id, CancellationToken token)
	{
		try
		{
			var result = await _scrapers
				.AsNoTracking()
				.SingleOrDefaultAsync(t => t.Id == id, token);

			if (result == null)
				return NotFoundError.From<ScraperId>(id);

			return new ScraperDto(result);
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}

	public async Task<Result<IEnumerable<ScraperDto>>> GetAllScheduledScrapers(PageNumber pageNumber, PageSize pageSize, CancellationToken token)
	{
		try
		{
			var results = await _scrapers
				.AsNoTracking()
				.Where(t => t.Scheduling != null && t.Status == ScraperStatus.Active)
				.OrderByDescending(t => t.CreatedOn)
				.Skip((pageNumber.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToListAsync(token);

			return results.Select(t => new ScraperDto(t)).ToList();
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}		
	}

	public async Task<Result<IEnumerable<ScraperDto>>> List(BrandId brandId, PageNumber pageNumber, PageSize pageSize, CancellationToken token)
	{
		try
		{
			var catalogIds = await _catalogs
				.AsNoTracking()
				.Where(c => c.BrandId == brandId)
				.Select(c => c.Id)
				.ToListAsync(token);

			var results = await _scrapers
				.AsNoTracking()
				.Where(t => catalogIds.Contains(t.CatalogId))
				.OrderByDescending(t => t.CreatedOn)
				.Skip((pageNumber.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToListAsync(token);

			return results.Select(t => new ScraperDto(t)).ToList();
		}
		catch (Exception exc)
		{
			return new UnexpectedError(exc);
		}
	}
}
