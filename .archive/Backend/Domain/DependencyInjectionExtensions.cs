using ITracker.Core.Domain;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions 
{
	public static IServiceCollection AddDomain(this IServiceCollection services)
	{
		services.AddScoped<ParsingEngine>();
		return services;
	}
}