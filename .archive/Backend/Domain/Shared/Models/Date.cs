namespace ITracker.Core.Domain;

public record Date(DateOnly Value) : ValueObject<DateOnly>(Value);
