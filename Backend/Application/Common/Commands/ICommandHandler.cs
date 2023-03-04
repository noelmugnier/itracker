namespace ITracker.Core.Application;

public interface IBaseCommand{}
public interface ICommand<TResponse> : IBaseCommand, MediatR.IRequest<Result<TResponse>>
{
}

public interface ICommand : IBaseCommand, MediatR.IRequest<Result>
{
}

public interface ICommandHandler<TRequest, TResponse> : MediatR.IRequestHandler<TRequest, Result<TResponse>>
	where TRequest : ICommand<TResponse>
{
}

public interface ICommandHandler<TRequest> : MediatR.IRequestHandler<TRequest, Result>
	where TRequest : ICommand
{
}