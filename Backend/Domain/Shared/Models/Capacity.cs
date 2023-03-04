namespace ITracker.Core.Domain;

public record Capacity : ValueObject<decimal>
{
	private Capacity():base(0){}
	
	public Capacity(decimal value, string unit) : base(value)
	{
		if(value <= 0)
			throw new DomainException(ErrorCode.CapacityMustBeGreaterThanZero);

		Unit = unit;
	}

	public string Unit { get; }
}