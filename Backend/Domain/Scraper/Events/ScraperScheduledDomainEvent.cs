namespace ITracker.Core.Domain;

public record ScraperScheduledDomainEvent(ScraperId ScraperId, Cron Cron) : DomainEvent;
