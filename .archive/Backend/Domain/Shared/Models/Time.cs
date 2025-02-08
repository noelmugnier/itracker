namespace ITracker.Core.Domain;

public record Time(TimeOnly Value) : ValueObject<TimeOnly>(Value);
