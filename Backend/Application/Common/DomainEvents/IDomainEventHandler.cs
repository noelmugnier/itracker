using MediatR;

namespace ITracker.Core.Application;

public record WrappedDomainEvent<T>(T Notification) : INotification where T: DomainEvent;

public interface IDomainEventHandler<T> : INotificationHandler<WrappedDomainEvent<T>> where T : DomainEvent
{
}

internal abstract class DomainEventHandler<T> : IDomainEventHandler<T> where T: DomainEvent
{
	public async Task Handle(WrappedDomainEvent<T> wrappedDomainEvent, CancellationToken token)
	{
		var result = await HandleEvent(wrappedDomainEvent.Notification, token);
		if (result.IsSuccess)
			return;
		
		throw new ApplicationException(result.Errors.Select(e => e.Message).Aggregate((a, b) => $"{a}, {b}"));
	}

	public abstract Task<Result> HandleEvent(T notification, CancellationToken token);
}