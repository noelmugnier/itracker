namespace ITracker.Core.Application;

public record ParserDto(string ElementSelector, IEnumerable<PropertySelectorDto>? Properties = null)
{
	public ParserDto() : this(string.Empty, new List<PropertySelectorDto>())
	{		
	}

	public ParserDto(Parser selector) 
		: this(selector?.ElementSelector?.Value ?? string.Empty, 
			selector?.Properties?.Select(property => new PropertySelectorDto(property)))
	{		
	}
}

public class ParserDtoValidator : AbstractValidator<ParserDto>
{
	public ParserDtoValidator()
	{
		RuleFor(x => x.ElementSelector).NotEmpty().WithErrorCode(ErrorCode.ElementSelectorCannotBeEmpty);

		When(x => x.Properties is not null, () => {
			RuleForEach(x => x.Properties).SetValidator(new PropertySelectorDtoValidator());
		});
    }
}
