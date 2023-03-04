namespace ITracker.Core.Application;

public interface IBrandReadRepository
{
	Task<Result<BrandDto>> FindById(BrandId id, CancellationToken token);
	Task<Result<IEnumerable<BrandDto>>> List(PageNumber pageNumber, PageSize pageSize, CancellationToken token);
}
