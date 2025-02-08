using System.Text.Json;
using ITracker.Core.Application;
using Newtonsoft.Json;

namespace ITracker.Adapters.Persistence;
 
public class OutboxIntegrationEvent
{
	private OutboxIntegrationEvent(){}

	public OutboxIntegrationEvent(IntegrationEvent integrationEvent)
	{
		Id = integrationEvent.Id;
		InsertedOn = DateTimeOffset.UtcNow;
		Type = integrationEvent.GetType().FullName!;
		Data = JsonConvert.SerializeObject(integrationEvent, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
	}

	public DateTimeOffset InsertedOn { get; }
	public Guid Id { get; }
	public string Type { get; private set; }
	public string Data { get; private set; }
	public DateTimeOffset? ProcessedOn { get; set; }
}