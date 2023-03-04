namespace ITracker.Core.Domain;

public class PersistenceError : UnexpectedError
{	
	public PersistenceError(string message): base(message)
	{
		CausedBy(message, new Exception(message));
	}

	public PersistenceError(Exception exception): base(exception.Message)
	{
		CausedBy(exception.Message, exception);
	}
}
