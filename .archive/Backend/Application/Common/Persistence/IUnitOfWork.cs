namespace ITracker.Core.Application;

public interface IUnitOfWork 
{
	public T Get<T>() where T: IRepository;
	public Task<Result> Commit(CancellationToken token);
}
