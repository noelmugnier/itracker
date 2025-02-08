namespace ITracker.Core.Domain;

public class UnexpectedError : Error
{	
	public UnexpectedError(string message): base(message)
	{
		CausedBy(message, new Exception(message));
	}

	public UnexpectedError(Exception exception): base(exception.Message)
	{
		CausedBy(exception.Message, exception);
	}
}
