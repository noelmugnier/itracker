namespace ITracker.Core.Domain;

public class DomainException : Exception
{
	public ErrorCode Code { get; }

	public DomainException(ErrorCode code, string? message = null) : base(message ?? code.ToString("G"))
	{
		Code = code;
	}
}
