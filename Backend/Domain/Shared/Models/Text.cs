namespace ITracker.Core.Domain;

public record Text(string Value) : ValueObject<string>(Value)
{
	public static Text From(string v)
	{
		return new Text(v);
	}
}

