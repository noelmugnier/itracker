namespace ITracker.Core.Domain;

public record ScraperActivatedDomainEvent(ScraperId ScraperId) : DomainEvent;
