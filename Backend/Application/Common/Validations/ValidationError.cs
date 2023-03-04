namespace ITracker.Core.Application;

public class ValidationError : Error
{	
	public ValidationError(ValidationException e) : base(e.Message)
	{
		CausedBy(e.Message, e);
	}
}
