namespace ITracker.Core.Domain;

public record PageNumber
{	
	private PageNumber(){}
	
	public PageNumber(int value)
	{
		if(value == 0)
			throw new DomainException(ErrorCode.PageNumberMustBeGreaterThanZero);
		
		Value = value;
	}

	public int Value { get; }

	public static PageNumber From(int value)
	{
		return new PageNumber(value);
	}

	public static PageNumber FromNullable(int? value)
	{
		if(!value.HasValue)
			return new PageNumber(1);

		return new PageNumber(value.Value);
	}
}

public static class PageNumberExtensions 
{
	public static int? ToNullable(this PageNumber? pageNumber)
	{
		return pageNumber?.Value;
	}
}
