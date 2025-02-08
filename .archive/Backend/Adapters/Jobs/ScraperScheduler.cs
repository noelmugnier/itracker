using ITracker.Core.Domain;
using FluentResults;
using Quartz;
using ITracker.Core.Application;

namespace ITracker.Adapters.Jobs;

public class ScraperScheduler : IScraperScheduler
{
	private readonly ISchedulerFactory _schedulerFactory;
	private readonly IScraperReadRepository _scraperReadRepository;

	public ScraperScheduler(
		IScraperReadRepository scraperReadRepository,
		ISchedulerFactory schedulerFactory)
	{
		_schedulerFactory = schedulerFactory;
		_scraperReadRepository = scraperReadRepository;
	}

	public async Task<Result> ScheduleScraper(Scraper scraper, CancellationToken token)
	{
		try
		{
			if(!scraper.CanBeScheduled)
				return Result.Ok();

			var scheduler = await _schedulerFactory.GetScheduler();
			await scheduler.UnscheduleJob(ExecuteScraperJob.GetTriggerIdentityKey(scraper.Id));

			var triggers = new List<ITrigger>();
			var job = await scheduler.GetJobDetail(ExecuteScraperJob.JobIdentityKey, token);
			if(job == null)
				job = JobBuilder.Create<ExecuteScraperJob>()
					.WithIdentity(ExecuteScraperJob.JobIdentityKey)
					.Build();
			else
				triggers.AddRange(await scheduler.GetTriggersOfJob(ExecuteScraperJob.JobIdentityKey, token));
			
			triggers.Add(TriggerBuilder.Create()
				.WithIdentity(ExecuteScraperJob.GetTriggerIdentityKey(scraper.Id))
				.WithSchedule(CronScheduleBuilder.CronSchedule(scraper.Scheduling!.Cron.Value))
				.ForJob(ExecuteScraperJob.JobIdentityKey)
				.UsingJobData(nameof(ExecuteScraperJob.ScraperId), scraper.Id.Value)
				.StartNow()
				.Build());

			await scheduler.ScheduleJob(job, triggers, true, token);
			return Result.Ok();
		}
		catch (Exception exc)
		{
			return Result.Fail(new UnexpectedError(exc));
		}
	}

	public async Task<Result> UnscheduleScraper(Scraper scraper, CancellationToken token)
	{
		try
		{
			var scheduler = await _schedulerFactory.GetScheduler();
			await scheduler.UnscheduleJob(ExecuteScraperJob.GetTriggerIdentityKey(scraper.Id), token);

			return Result.Ok();
		}
		catch (Exception exc)
		{
			return Result.Fail(new UnexpectedError(exc));
		}
	}
}
