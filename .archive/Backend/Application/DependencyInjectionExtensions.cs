using ITracker.Core.Application;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions 
{
	public static IServiceCollection AddApplication(this IServiceCollection services)
	{
		services.AddDomain();
		
		services.AddScoped<IIntegrationEventsPublisher, IntegrationEventsPublisher>();
		services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
		services.AddScoped<ICommandMediator, CommandMediator>();
		services.AddScoped<IQueryMediator, QueryMediator>();
				
    	services.AddValidatorsFromAssemblyContaining<CreateBrandScraperCommandValidator>();

		services.AddMediatR(config => {	
			var commandWithResultType = typeof(IBaseCommand);
			var types = typeof(ICommand).Assembly.GetTypes().Where(t => commandWithResultType.IsAssignableFrom(t) && !t.IsInterface);
			foreach(var type in types)
			{
				if(typeof(ICommand).IsAssignableFrom(type))
				{
					var pipelineType = typeof(MediatR.IPipelineBehavior<,>).MakeGenericType(type, typeof(Result));
					config.AddBehavior(pipelineType, typeof(CommandLoggingBehavior<>).MakeGenericType(type));
					config.AddBehavior(pipelineType, typeof(CommandValidationBehavior<>).MakeGenericType(type));
				}
				else
				{
					var commandReturnType = type.GetInterfaces().FirstOrDefault(i => typeof(IBaseCommand).IsAssignableFrom(i))?.GenericTypeArguments.FirstOrDefault();
					if(commandReturnType == null)
						continue;

					var pipelineType = typeof(MediatR.IPipelineBehavior<,>).MakeGenericType(type, typeof(Result<>).MakeGenericType(commandReturnType));
					config.AddBehavior(pipelineType, typeof(CommandLoggingBehavior<,>).MakeGenericType(type, commandReturnType));
					config.AddBehavior(pipelineType, typeof(CommandValidationBehavior<,>).MakeGenericType(type, commandReturnType));
				}
			}

			config.RegisterServicesFromAssemblyContaining<IUnitOfWork>();
			config.NotificationPublisherType = typeof(MediatR.NotificationPublishers.TaskWhenAllPublisher); 
		});

		return services;
	}
}