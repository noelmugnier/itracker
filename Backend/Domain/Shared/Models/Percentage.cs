namespace ITracker.Core.Domain;

public record Percentage(decimal Value) : ValueObject<decimal>(Value);
