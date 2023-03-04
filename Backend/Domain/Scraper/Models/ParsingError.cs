namespace ITracker.Core.Domain;

public record ParsingError(string? Message, ErrorCode Code);
