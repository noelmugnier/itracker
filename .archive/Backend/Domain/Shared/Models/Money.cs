namespace ITracker.Core.Domain;

public record Money(decimal Value, string Currency) : ValueObject<decimal>(Value);
