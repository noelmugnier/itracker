namespace ITracker.Core.Domain;

public record IntegerNumber(long Value) : ValueObject<long>(Value);
