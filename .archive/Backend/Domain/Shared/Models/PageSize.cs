namespace ITracker.Core.Domain;

public record PageSize
{	
	private PageSize(){}
	
	public PageSize(int value)
	{
		if(value == 0)
			throw new DomainException(ErrorCode.PageSizeMustBeGreaterThanZero);
		
		Value = value;
	}

	public int Value { get; }

	public static PageSize From(int value)
	{
		return new PageSize(value);
	}

	public static PageSize? FromNullable(int? value)
	{
		if(!value.HasValue)
			return null;

		return new PageSize(value.Value);
	}
}

public static class PageSizeExtensions 
{
	public static int? ToNullable(this PageSize? pageSize)
	{
		return pageSize?.Value;
	}
}
