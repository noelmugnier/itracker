namespace ITracker.Core.Application;

public static class ValidationRuleBuilderExtensions 
{    
	public static IRuleBuilderOptions<T, TProperty> WithErrorCode<T, TProperty>(this IRuleBuilderOptions<T, TProperty> rule, ErrorCode errorCode)
    {
        return rule.WithErrorCode(errorCode.ToString("G"));
    }
}