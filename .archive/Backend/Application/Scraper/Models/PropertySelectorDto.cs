namespace ITracker.Core.Application;

public record PropertySelectorDto(string PropertyName, string ValueSelector)
{
	public PropertySelectorDto() : this(string.Empty, string.Empty)
	{		
	}

	public PropertySelectorDto(PropertyToParse propertyToParse) 
		: this(propertyToParse.Name.Value, propertyToParse.ValueSelector.Value)
	{
	}
}

public class PropertySelectorDtoValidator : AbstractValidator<PropertySelectorDto>
{
	public PropertySelectorDtoValidator()
	{
		RuleFor(x => x.PropertyName).NotEmpty().WithErrorCode(ErrorCode.PropertyNameCannotBeEmpty);
		RuleFor(x => x.ValueSelector).NotEmpty().WithErrorCode(ErrorCode.PropertySelectorCannotBeEmpty);
    }
}