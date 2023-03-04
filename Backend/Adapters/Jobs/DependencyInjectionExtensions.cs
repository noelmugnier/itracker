using ITracker.Adapters.Jobs;
using ITracker.Core.Application;
using ITracker.Core.Domain;
using Quartz;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions 
{
	public static IServiceCollection AddJobs(this IServiceCollection services)
	{        
		services.AddTransient<ExecuteScraperJob>();
		services.AddTransient<PublishOutboxIntegrationEventsJob>();

		services.AddScoped<IScraperScheduler, ScraperScheduler>();

		services.AddQuartz(q =>
		{
			q.UseMicrosoftDependencyInjectionJobFactory();	

			q.AddJob<PublishOutboxIntegrationEventsJob>(
				opts => opts
					.WithIdentity(PublishOutboxIntegrationEventsJob.JobIdentityKey)
					.DisallowConcurrentExecution());
				
			q.AddTrigger(
				opts => opts
					.ForJob(PublishOutboxIntegrationEventsJob.JobIdentityKey)
					.WithIdentity(PublishOutboxIntegrationEventsJob.TriggerIdentityKey)
					.WithCronSchedule(PublishOutboxIntegrationEventsJob.CronExpression));

			q.AddJob<RemoveProcessedIntegrationEventsJob>(
				opts => opts
					.WithIdentity(RemoveProcessedIntegrationEventsJob.JobIdentityKey)
					.DisallowConcurrentExecution());
				
			q.AddTrigger(
				opts => opts
					.ForJob(RemoveProcessedIntegrationEventsJob.JobIdentityKey)
					.WithIdentity(RemoveProcessedIntegrationEventsJob.TriggerIdentityKey)
					.WithCronSchedule(RemoveProcessedIntegrationEventsJob.CronExpression));
		});

		services.AddQuartzServer(options =>
		{
			options.WaitForJobsToComplete = true;
		});

		return services;
	}

	public static async Task InitScheduledScrapers(this IServiceProvider serviceProvider)
	{		
		using var scope = serviceProvider.CreateScope();
		var result = await scope.ServiceProvider.GetRequiredService<ICommandMediator>().Execute(new InitScheduledScraperCommand(), CancellationToken.None);
		if(result.IsFailed)
			throw new InvalidOperationException(result.Errors.First().Message);
	}
}