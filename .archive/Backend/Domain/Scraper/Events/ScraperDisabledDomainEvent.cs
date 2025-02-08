namespace ITracker.Core.Domain;

public record ScraperDisabledDomainEvent(ScraperId ScraperId) : DomainEvent;
