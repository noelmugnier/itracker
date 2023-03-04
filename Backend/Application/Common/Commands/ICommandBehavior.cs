namespace ITracker.Core.Application;

public interface ICommandBehavior<TRequest, TResponse> : MediatR.IPipelineBehavior<TRequest, Result<TResponse>>
	where TRequest : ICommand<TResponse>
{
}

public interface ICommandBehavior<TRequest> : MediatR.IPipelineBehavior<TRequest, Result>
	where TRequest : ICommand
{
}
