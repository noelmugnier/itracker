namespace ITracker.Core.Domain;

public record Email(string Value) : ValueObject<string>(Value);
