namespace ITracker.Core.Domain;

public class UnexpectedException : Exception
{
	public UnexpectedException(string message, Exception innerException) : base(message ?? innerException?.Message, innerException)
	{		
	}
}
