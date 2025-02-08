namespace ITracker.Core.Domain;

public record DecimalNumber(decimal Value) : ValueObject<decimal>(Value);
