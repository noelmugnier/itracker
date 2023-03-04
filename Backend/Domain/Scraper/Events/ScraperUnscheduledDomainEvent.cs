namespace ITracker.Core.Domain;

public record ScraperUnscheduledDomainEvent(ScraperId ScraperId) : DomainEvent;