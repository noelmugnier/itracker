namespace ITracker.Core.Application;

public interface ICommandMediator
{
	Task<Result> Execute(ICommand command, CancellationToken token);
	Task<Result<TResponse>> Execute<TResponse>(ICommand<TResponse> command, CancellationToken token);
}

public class CommandMediator : ICommandMediator
{
	private MediatR.IMediator _mediator;

	public CommandMediator(MediatR.IMediator mediator)
	{
		_mediator = mediator;
	}
	
	public Task<Result> Execute(ICommand command, CancellationToken token)
	{
		return _mediator.Send(command, token);
	}

	public Task<Result<TResponse>> Execute<TResponse>(ICommand<TResponse> command, CancellationToken token)
	{
		return _mediator.Send(command, token);
	}
}
