namespace ITracker.Core.Domain;

public abstract record ValueObject<T>(T Value);

public record Rating(decimal Value, int MaxValue, int? Count) : ValueObject<decimal>(Value);
