using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints.Swagger;

namespace ITracker.Adapters.Api;

public class ApiAdapter
{
	private WebApplication _app;

	public ApiAdapter(string[] args, Func<WebApplicationBuilder, WebApplicationBuilder> configure)
	{
		var builder = WebApplication.CreateBuilder(args);

		builder = configure(builder);
	   
	   builder.Services.AddCors(options =>
	   {
		   options.AddPolicy("CorsPolicy", builder =>
		   {
			   builder.AllowAnyOrigin()
				   .AllowAnyMethod()
				   .AllowAnyHeader();
		   });
	   });

		builder.Services.AddAuthentication();
		builder.Services.AddAuthorization();		
		builder.Services.AddFastEndpoints();
		builder.Services.AddSwaggerDoc(serializerSettings: x =>
		{
			x.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			x.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
		}, tagIndex: 0, shortSchemaNames: true);

		builder.Services.AddApplication();

		_app = builder.Build();

		_app.UseCors("CorsPolicy");
		_app.UseDefaultExceptionHandler();	
		_app.UseHttpsRedirection();
		_app.UseAuthorization();
		_app.UseAuthentication();	
		_app.UseFastEndpoints(c => {			
			c.Serializer.Options.Converters.Add(new JsonStringEnumConverter());
			c.Serializer.Options.PropertyNameCaseInsensitive = true;
			c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
			c.Serializer.Options.NumberHandling = JsonNumberHandling.AllowReadingFromString;
			c.Serializer.Options.AllowTrailingCommas = true;
			c.Serializer.Options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
			
    		c.Endpoints.RoutePrefix = "api";
			c.Endpoints.ShortNames = true;
		});
		_app.UseSwaggerGen();
	}

	public async Task StartAsync(Func<WebApplication, Task<WebApplication>> use)
	{
		await use(_app);
		await _app.RunAsync();
	}
}
