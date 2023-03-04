namespace ITracker.Core.Application;

public record PropertySchemaDto(string PropertyName, string DisplayName, ValueKind ValueType, bool Required = true, bool Tracked = false);