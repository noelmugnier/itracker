namespace ITracker.Core.Domain;

public interface IRepository { }
public interface IRepository<T, TId> : IRepository 
	where T : Entity<TId>
    where TId : notnull
{
	Task<OneOf<T, NotFoundError, UnexpectedError>> Get(TId id, CancellationToken token);
}
