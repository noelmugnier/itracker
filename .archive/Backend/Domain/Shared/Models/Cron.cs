namespace ITracker.Core.Domain;

public record Cron
{
	private const string _regex = @"^(?<seconds>[\d+|\*|\,|\-|\/]+)\s(?<minutes>[\d+|\*|\,|\-|\/]+)\s(?<hours>[\d+|\*|\,|\-|\/]+)\s(?<dayofmonth>[\d+|\*|\,|\-|\/|\?|L|W]+)\s(?<months>[\d+|\*|\,|\-|\/]+)\s(?<dayofweek>[\d+|\*|\,|\-|\/|\?|L|\#]+)(\s(?<year>[\d+|\*|\,|\-|\/]+))?$";
	
	private Cron(){}
	
	public Cron(string value)
	{
		if(value is null)
			throw new DomainException(ErrorCode.CronExpressionIsRequired);

		if(string.IsNullOrWhiteSpace(value))
			throw new DomainException(ErrorCode.CronExpressionCannotBeEmpty);
			
		if (!Regex.IsMatch(value, _regex))
			throw new DomainException(ErrorCode.CronExpressionIsInvalid);
		
		Value = value;
	}

	public string Value { get; }

	public static Cron From(string expression)
	{
		return new Cron(expression);
	}

	public static bool IsValid(string value)
	{
		return Regex.IsMatch(value, _regex);
	}
}
