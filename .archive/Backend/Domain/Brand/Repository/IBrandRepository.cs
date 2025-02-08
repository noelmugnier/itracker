namespace ITracker.Core.Domain;

public interface IBrandRepository : IRepository<Brand, BrandId>
{
	Task<Result> Insert(Brand brand, CancellationToken token);
	Task<Result> Update(Brand brand, CancellationToken token);
	Task<Result> Delete(Brand brand, CancellationToken token);
}