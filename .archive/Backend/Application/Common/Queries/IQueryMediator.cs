namespace ITracker.Core.Application;

public interface IQueryMediator
{
	Task<Result<TResponse>> Retrieve<TResponse>(IQuery<TResponse> query, CancellationToken token);
}

public class QueryMediator : IQueryMediator
{
	private MediatR.IMediator _mediator;

	public QueryMediator(MediatR.IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task<Result<TResponse>> Retrieve<TResponse>(IQuery<TResponse> query, CancellationToken token)
	{
		return _mediator.Send(query, token);
	}
}