namespace ITracker.Core.Domain;

public class NotFoundException : Exception
{
	public NotFoundException(string message) : base(message)
	{		
	}
}
