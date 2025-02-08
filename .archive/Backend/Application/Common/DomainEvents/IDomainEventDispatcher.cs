using MediatR;

namespace ITracker.Core.Application;

public interface IDomainEventDispatcher
{
	Task<Result> DispatchEvents<T>(IEnumerable<T> domainEvents, CancellationToken token) where T : DomainEvent;
}

public class DomainEventDispatcher : IDomainEventDispatcher
{
	private readonly IMediator _mediator;

	public DomainEventDispatcher(IMediator mediator)
	{
		_mediator = mediator;
	}

	public async Task<Result> DispatchEvents<T>(IEnumerable<T> domainEvents, CancellationToken token) where T : DomainEvent
	{
		try 
		{
			foreach(var domainEvent in domainEvents.ToList())
			{				
				var wrappedNotificationType = typeof(WrappedDomainEvent<>).MakeGenericType(domainEvent.GetType());
				var wrappedNotification = Activator.CreateInstance(wrappedNotificationType, domainEvent);
				if(wrappedNotification == null)
					return Result.Fail($"Failed to create WrappedDomainEvent<{domainEvent.GetType().Name}>");
				
				await _mediator.Publish(wrappedNotification, token);
			}
				
			return Result.Ok();
		}
		catch (Exception e)
		{
			return Result.Fail(e.Message);
		}
	}
}
