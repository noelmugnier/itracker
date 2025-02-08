namespace ITracker.Core.Domain;

public record BrandCreatedDomainEvent(BrandId BrandId) : DomainEvent;