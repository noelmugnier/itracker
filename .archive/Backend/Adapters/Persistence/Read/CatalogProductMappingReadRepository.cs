using ITracker.Core.Domain;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using ITracker.Core.Application;

namespace ITracker.Adapters.Persistence;

public class CatalogProductMappingReadRepository : ICatalogProductMappingReadRepository
{
	private readonly DbSet<Product> _products;
	private readonly DbSet<CatalogProductMapping> _mappings;

	public CatalogProductMappingReadRepository(AppDbContext context)
	{
		_products = context.Set<Product>();
		_mappings = context.Set<CatalogProductMapping>();
	}

	public async Task<Result<IEnumerable<ProductToMapDto>>> ListPendingProducts(CatalogId sourceCatalogId, CatalogId targetCatalogId, PageNumber pageNumber, PageSize pageSize, CancellationToken token)
	{
		try
		{
			var allProducts = 
				from p in _products
				from m in _mappings.Where(m => m.SourceCatalogId == p.CatalogId && m.SourceProductId == p.Id).DefaultIfEmpty()
				where p.CatalogId == sourceCatalogId && m.TargetCatalogId == null
				orderby p.CreatedOn ascending
				select new ProductToMapDto(sourceCatalogId.Value, p.Id.Value, p.Name.Value, targetCatalogId.Value, null);

			return await allProducts
				.Skip((pageNumber.Value - 1) * pageSize.Value)
				.Take(pageSize.Value)
				.ToListAsync(token);
		}
		catch (Exception exc)
		{			
			return new UnexpectedError(exc);
		}
	}
}
