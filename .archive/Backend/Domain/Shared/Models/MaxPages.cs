namespace ITracker.Core.Domain;

public record MaxPages
{	
	private MaxPages(){}
	
	public MaxPages(int value)
	{		
		if(value == 0)
			throw new DomainException(ErrorCode.MaxPagesMustBeGreaterThanZero);

		Value = value;
	}

	public int Value { get; }

	public static MaxPages From(int value)
	{
		return new MaxPages(value);
	}
}
