using ITracker.Adapters.Api;

var api = new ApiAdapter(args, builder =>
{	
	builder.Services.AddContentRetriever();
	builder.Services.AddContentParser();
	builder.Services.AddNpgsqlPersistence(builder.Configuration);
	builder.Services.AddJobs();

	return builder;
});

await api.StartAsync(async app => {
	await app.Services.EnsurePersistenceExistance();
	await app.Services.InitScheduledScrapers();
	return app;
});
