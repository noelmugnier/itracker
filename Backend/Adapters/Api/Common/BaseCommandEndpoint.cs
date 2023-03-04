namespace ITracker.Adapters.Api;

public abstract class BaseCommandEndpoint<TRequest> : Endpoint<TRequest> where TRequest : notnull
{
	protected readonly ICommandMediator Mediator;

	protected BaseCommandEndpoint(ICommandMediator commandMediator)
	{
		Mediator = commandMediator;
	}	

	public async override Task HandleAsync(TRequest request, CancellationToken token)
	{        
		var result = await Handle(request, token);
		var response = result.ToResponse();
		response.Switch(
			async success => await SendNoContentAsync(token),
			async notFound => await SendErrorsAsync(404, token),
			async validations => await SendErrorsAsync(422, token),
			async error => await SendErrorsAsync(400, token)
		);
	}

	public abstract Task<Result> Handle(TRequest request, CancellationToken token);
}

public abstract class BaseCommandEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse> where TRequest : notnull
{
	protected readonly ICommandMediator Mediator;

	protected BaseCommandEndpoint(ICommandMediator commandMediator)
	{
		Mediator = commandMediator;
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


