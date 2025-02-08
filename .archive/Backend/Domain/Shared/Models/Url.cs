namespace ITracker.Core.Domain;

public record Url(string Value) : ValueObject<string>(Value)
{
	public static Url From(string v)
	{
		return new Url(v);
	}
}

