using ITracker.Adapters.ContentRetriever;
using ITracker.Core.Domain;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions 
{
	public static IServiceCollection AddContentRetriever(this IServiceCollection services)
	{        
		services.AddScoped<IContentRetriever, PlaywrightContentRetrieverAdapter>();
		return services;
	}
}