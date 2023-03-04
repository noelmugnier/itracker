using System.Reflection;
using FluentResults;
using ITracker.Core.Application;
using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ITracker.Adapters.Persistence;

public class OutboxIntegrationEventsProcessor : IOutboxIntegrationEventsProcessor
{
	private readonly IServiceScopeFactory _context;

	public OutboxIntegrationEventsProcessor(IServiceScopeFactory context)
	{
		_context = context;
	}

	public async Task<Result> PublishPendingEvents(CancellationToken token)
	{
		try
		{
			using var scope = _context.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			var outboxEvents = await dbContext.Set<OutboxIntegrationEvent>().Where(oie => !oie.ProcessedOn.HasValue).ToListAsync(token);

			var errors = new List<IError>();
			foreach(var outboxEvent in outboxEvents)
			{
				var type = Assembly.GetAssembly(typeof(IntegrationEvent))!.GetType(outboxEvent.Type);
				if(type == null)
				{
					errors.Add(new UnexpectedError($"Failed to load outboxevent type : {outboxEvent.Type}"));
					continue;
				}

				var integrationEvent = JsonConvert.DeserializeObject(outboxEvent.Data, type, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Objects });
				if(integrationEvent == null)
				{
					errors.Add(new UnexpectedError($"Failed to deserialize outboxevent data : {outboxEvent.Type}"));
					continue;
				}

				using var innerScope = scope.ServiceProvider.CreateScope();
				var integrationEventsPublisher = innerScope.ServiceProvider.GetRequiredService<IIntegrationEventsPublisher>();
				var result = await integrationEventsPublisher.Publish((IntegrationEvent)integrationEvent, token);
				if(result.IsFailed)
				{
					errors.AddRange(result.Errors);
					continue;
				}
					
				outboxEvent.ProcessedOn = DateTimeOffset.UtcNow;
				await dbContext.SaveChangesAsync(token);
			}	

			if(errors.Any())
				return Result.Fail(errors);

			return Result.Ok();
		}
		catch(Exception exc)
		{
			return Result.Fail(new UnexpectedError(exc));
		}
	}

	public async Task<Result> RemoveProcessedEvents(CancellationToken token)
	{
		try
		{
			const int take = 100;
			var skip = 0;

			using var scope = _context.CreateScope();
			var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
			var outboxEvents = await dbContext.Set<OutboxIntegrationEvent>()
				.Where(oie => oie.ProcessedOn.HasValue)
				.OrderBy(oie => oie.ProcessedOn)
				.Skip(skip)
				.Take(take)
				.ToListAsync(token);

			while(outboxEvents.Any())
			{
				dbContext.Set<OutboxIntegrationEvent>().RemoveRange(outboxEvents);
				await dbContext.SaveChangesAsync(token);

				skip += take;
				outboxEvents = await dbContext.Set<OutboxIntegrationEvent>()
					.Where(oie => oie.ProcessedOn.HasValue)
					.OrderBy(oie => oie.ProcessedOn)
					.Skip(skip)
					.Take(take)
					.ToListAsync(token);
			}	

			return Result.Ok();
		}
		catch(Exception exc)
		{
			return Result.Fail(new UnexpectedError(exc));
		}
	}
}