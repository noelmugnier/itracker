using ITracker.Adapters.ContentParser;
using ITracker.Core.Domain;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions 
{
	public static IServiceCollection AddContentParser(this IServiceCollection services)
	{        
		services.AddScoped<IContentParser, AngleSharpParserAdapter>();
		return services;
	}
}