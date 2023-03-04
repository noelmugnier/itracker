namespace ITracker.Core.Application;

public interface IOutboxIntegrationEventsProcessor 
{
	public Task<Result> PublishPendingEvents(CancellationToken token);
	public Task<Result> RemoveProcessedEvents(CancellationToken token);
}
