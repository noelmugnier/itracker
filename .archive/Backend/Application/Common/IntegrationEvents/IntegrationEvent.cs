namespace ITracker.Core.Application;

public abstract record IntegrationEvent : MediatR.INotification
{	
	public Guid Id { get; private set; } = Guid.NewGuid();
	public DateTimeOffset OccuredOn { get; private set; } = DateTimeOffset.UtcNow;
}
