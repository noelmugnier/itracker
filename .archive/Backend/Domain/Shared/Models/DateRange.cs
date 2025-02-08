namespace ITracker.Core.Domain;

public record DateRange
{	
	private DateRange(){}
	
	public DateRange(DateTimeOffset start, DateTimeOffset end)
	{
		if(start == default)
			throw new DomainException(ErrorCode.DateRangeStartDateIsInvalid);

		if(end == default)
			throw new DomainException(ErrorCode.DateRangeEndDateIsInvalid);

		if(end < start)
			throw new DomainException(ErrorCode.DateRangeStartDateMustBeLowerThanEndDate);

		Start = start;
		End = end;
	}

	public DateTimeOffset Start { get; }
	public DateTimeOffset End { get; }
}