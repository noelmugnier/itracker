namespace ITracker.Core.Domain;

public record ScraperCreatedDomainEvent(ScraperId ScraperId) : DomainEvent;
