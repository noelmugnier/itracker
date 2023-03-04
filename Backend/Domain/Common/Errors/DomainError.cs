namespace ITracker.Core.Domain;

public class DomainError : Error
{
	public ErrorCode Code { get; }
	public DomainError(ErrorCode code, string? message = null): base(message ?? $"{code:G}")
	{
		Code = code;
		CausedBy(message, new DomainException(code));
	}
}
