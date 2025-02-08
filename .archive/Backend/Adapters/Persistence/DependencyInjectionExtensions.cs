using ITracker.Adapters.Persistence;
using ITracker.Core.Application;
using ITracker.Core.Domain;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions 
{
	public static IServiceCollection AddNpgsqlPersistence(this IServiceCollection services, IConfiguration configuration)
	{        
		services.AddDbContext<IUnitOfWork, AppDbContext>(c => {
			c.UseNpgsql(configuration.GetConnectionString("pgsql"));
		});

		services.AddScoped<IOutboxIntegrationEventsProcessor, OutboxIntegrationEventsProcessor>();

		services.AddScoped<IBrandReadRepository, BrandReadRepository>();

		services.AddScoped<IScraperReadRepository, ScraperReadRepository>();
		services.AddScoped<IParsingResultReadRepository, ParsingResultReadRepository>();

		services.AddScoped<ICatalogReadRepository, CatalogReadRepository>();
		services.AddScoped<IProductReadRepository, ProductReadRepository>();
		services.AddScoped<IProductHistoryReadRepository, ProductHistoryReadRepository>();
		services.AddScoped<ICatalogProductMappingReadRepository, CatalogProductMappingReadRepository>();

		services.AddScoped<IScraperFieldsRetriever, ScraperFieldsRetriever>();

		return services;
	}

	public static IServiceCollection AddInMemoryPersistence(this IServiceCollection services)
	{        
		var conn = new SqliteConnection("Filename=:memory:");
		conn.Open();

		services.AddDbContext<IUnitOfWork, AppDbContext>(c => {
			c.UseSqlite(conn);
		});

		services.AddScoped<IOutboxIntegrationEventsProcessor, OutboxIntegrationEventsProcessor>();
		services.AddScoped<IScraperReadRepository, ScraperReadRepository>();
		services.AddScoped<IBrandReadRepository, BrandReadRepository>();
		services.AddScoped<IParsingResultReadRepository, ParsingResultReadRepository>();

		return services;
	}

	public static async Task EnsurePersistenceExistance(this IServiceProvider serviceProvider)
	{		
		using var scope = serviceProvider.CreateScope();
			await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
	}
}