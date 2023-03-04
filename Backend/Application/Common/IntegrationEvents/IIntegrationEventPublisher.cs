using MediatR;

namespace ITracker.Core.Application;

public interface IIntegrationEventsPublisher
{
	public Task<Result> Publish<T>(T integrationEvent, CancellationToken token) where T : IntegrationEvent;
}

internal class IntegrationEventsPublisher : IIntegrationEventsPublisher
{
	private readonly IMediator _mediator;

	public IntegrationEventsPublisher(IMediator mediator)
	{
		_mediator = mediator;
	}

	public async Task<Result> Publish<T>(T integrationEvent, CancellationToken token) where T : IntegrationEvent
	{
		try
		{
			await _mediator.Publish(integrationEvent, token);
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(new UnexpectedError(e));
		}
	}
}