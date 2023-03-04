namespace ITracker.Core.Application.Tests;

public class InitScheduledScraperTests 
{
	[Fact]
	public async void Should_Scheduled_Scrapers()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.AddScheduledScraper(brandId, catalogId, "* * * ? * * *", out var scraperId)
			.Build();
		var scheduler = new FakeScraperScheduler();
		var command = new InitScheduledScraperCommand();
		
		var result = await new InitScheduledScraperCommandHandler(scheduler, uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();
		scheduler.ScheduledScrapers.Should().ContainSingle();
	}

	[Fact]
	public async void Should_Not_Scheduled_Disabled_Scrapers()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.AddScheduledScraper(brandId, catalogId, "* * * ? * * *", out var scraperId, ScraperStatus.Disabled)
			.Build();
		var scheduler = new FakeScraperScheduler();
		var command = new InitScheduledScraperCommand();
		
		var result = await new InitScheduledScraperCommandHandler(scheduler, uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();
		scheduler.ScheduledScrapers.Should().BeEmpty();
	}
	
	[Fact]
	public async void Should_Not_Scheduled_Scrapers_RequiringReview()
	{
		var uow = new TestDatabaseBuilder()
			.AddBrand(out var brandId)
			.AddCatalog(brandId, out var catalogId)
			.AddScheduledScraper(brandId, catalogId, "* * * ? * * *", out var scraperId, ScraperStatus.ReviewRequired)
			.Build();
		var scheduler = new FakeScraperScheduler();
		var command = new InitScheduledScraperCommand();

		var result = await new InitScheduledScraperCommandHandler(scheduler, uow).Handle(command, CancellationToken.None);

		result.Should().BeSuccess();
		scheduler.ScheduledScrapers.Should().BeEmpty();
	}
}