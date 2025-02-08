namespace ITracker.Core.Application;

public interface IQueryBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, Result<TResponse>>
	where TRequest : IQuery<TResponse>
{
}