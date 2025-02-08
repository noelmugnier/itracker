namespace ITracker.Core.Application;

public record SchedulingDto(string CronExpression)
{
	public SchedulingDto() : this(string.Empty)
	{		
	}

	public SchedulingDto(Scheduling configuration)
		:this(configuration.Cron.Value)
	{		
	}
}

public class SchedulingDtoValidator : AbstractValidator<SchedulingDto>
{
	public SchedulingDtoValidator()
	{
		RuleFor(x => x.CronExpression).NotEmpty().WithErrorCode(ErrorCode.CronExpressionCannotBeEmpty);
		RuleFor(x => x.CronExpression).Must(x => Cron.IsValid(x)).WithErrorCode(ErrorCode.CronExpressionIsInvalid);
    }
}
