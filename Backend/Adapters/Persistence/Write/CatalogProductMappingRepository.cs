using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;

namespace ITracker.Adapters.Persistence;

public class CatalogProductMappingRepository : ICatalogProductMappingRepository
{
	private readonly DbSet<CatalogProductMapping> _set;

	public CatalogProductMappingRepository(DbContext context)
	{
		_set = context.Set<CatalogProductMapping>();
	}

	public async Task<Result> Insert(CatalogProductMapping mapping, CancellationToken token)
	{
		try
		{
			await _set.AddAsync(mapping, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}
	}

	public async Task<Result> Delete(CatalogId sourceCatalogId, ProductId sourceProductId, CatalogId targetCatalogId, ProductId targetProductId, CancellationToken token)
	{
		try
		{
			var mapping = await _set.SingleOrDefaultAsync(s => 
				s.SourceCatalogId == sourceCatalogId
				&& s.SourceProductId == sourceProductId
				&& s.TargetCatalogId == targetCatalogId
				&& s.TargetProductId == targetProductId, token);

			if(mapping == null)
				return Result.Ok();

			_set.Remove(mapping);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}
	}
}
