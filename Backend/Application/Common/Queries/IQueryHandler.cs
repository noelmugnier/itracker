namespace ITracker.Core.Application;

public interface IBaseQuery{}
public interface IQuery<TResponse> : IBaseQuery, MediatR.IRequest<Result<TResponse>>
{
}

public interface IQueryHandler<TQuery, TResponse> : MediatR.IRequestHandler<TQuery, Result<TResponse>>
	where TQuery : IQuery<TResponse>
{
}
