namespace ITracker.Core.Domain;

public class NotFoundError : Error
{
	public NotFoundError(string id): base($"{id} not found")
	{
		CausedBy($"{id} not found", new NotFoundException($"{id} not found"));
	}

	public static NotFoundError From<T>(T id)
	{
		return new NotFoundError(id.ToString());
	}
}
