namespace ITracker.Core.Domain;

public record Quantity(long Value) : ValueObject<long>(Value);
