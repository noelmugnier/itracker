namespace ITracker.Core.Domain;

public record Scheduling
{
	private Scheduling(){}

	public Scheduling(Cron cron)
	{
		Cron = cron;
	}

	public Cron Cron { get; }
}

