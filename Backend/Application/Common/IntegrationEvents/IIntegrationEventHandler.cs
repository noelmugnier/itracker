using MediatR;

namespace ITracker.Core.Application;

public interface IIntegrationEventHandler<T> : INotificationHandler<T> where T : IntegrationEvent
{
}

internal abstract class IntegrationEventHandler<T> : IIntegrationEventHandler<T> where T : IntegrationEvent
{
	public async Task Handle(T integrationEvent, CancellationToken token)
	{
		var result = await HandleEvent(integrationEvent, token);
		if (result.IsSuccess)
			return;
		
		throw new ApplicationException(result.Errors.Select(e => e.Message).Aggregate((a, b) => $"{a}, {b}"));
	}

	public abstract Task<Result> HandleEvent(T integrationEvent, CancellationToken token);
}
