namespace ITracker.Core.Application;

public record PaginationDto(string PageNumberParameterName, int MaxPages, int? PageSize = null, string? PageSizeParameterName = null)
{
	public PaginationDto() : this(string.Empty, default)
	{		
	}

	public PaginationDto(Pagination configuration) 
		: this(configuration.PageNumberParameterName.Value, configuration.MaxPages.Value, configuration.PageSize?.Value, configuration.PageSizeParameterName?.Value)
	{		
	}
};

public class PaginationDtoValidator : AbstractValidator<PaginationDto>
{
	public PaginationDtoValidator()
	{
		RuleFor(x => x.PageNumberParameterName).NotEmpty().WithErrorCode(ErrorCode.ParameterNameCannotBeEmpty);
		RuleFor(x => (int)x.MaxPages).GreaterThan(0).WithErrorCode(ErrorCode.MaxPagesMustBeGreaterThanZero);
		When(x => x.PageSize is not null, () => {
			RuleFor(x => (int)x.PageSize!).GreaterThan(0).WithErrorCode(ErrorCode.PageSizeMustBeGreaterThanZero);
		});
		When(x => x.PageSizeParameterName is not null, () => {
			RuleFor(x => x.PageSizeParameterName).NotEmpty().WithErrorCode(ErrorCode.ParameterNameCannotBeEmpty);
		});
    }
}