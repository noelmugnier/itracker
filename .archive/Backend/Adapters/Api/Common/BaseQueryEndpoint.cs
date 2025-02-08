namespace ITracker.Adapters.Api;

public abstract class BaseQueryEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse> where TRequest : notnull
{
	protected readonly IQueryMediator Mediator;

	protected BaseQueryEndpoint(IQueryMediator queryMediator)
	{
		Mediator = queryMediator;
	}

	public async override Task HandleAsync(TRequest request, CancellationToken token)
	{        		
		var result = await Handle(request, token);
		var response = result.ToResponse<TResponse>();
		response.Switch(
			async value => await SendAsync(value, 200, token),
			async notFound => await SendErrorsAsync(404, token),
			async validations => await SendErrorsAsync(422, token),
			async error => await SendErrorsAsync(400, token)
		);
	}

	public abstract Task<Result<TResponse>> Handle(TRequest request, CancellationToken token);
}


