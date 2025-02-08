namespace ITracker.Core.Domain;

public record PropertyFieldSchema(PropertyName PropertyName, ValueKind ValueType, bool Required, bool Tracked);
