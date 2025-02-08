namespace ITracker.Core.Application;

public record ProductSchemaDto(string IdentifierDisplayName, string NameDisplayName, string PriceDisplayName, IEnumerable<PropertySchemaDto> Fields);

public class ProductSchemaDtoValidator : AbstractValidator<ProductSchemaDto>
{
	public ProductSchemaDtoValidator()
	{
		RuleFor(x => x.IdentifierDisplayName).NotEmpty().WithErrorCode(ErrorCode.ProductIdentifierDisplayNameIsRequired);
		RuleFor(x => x.NameDisplayName).NotEmpty().WithErrorCode(ErrorCode.ProductNameDisplayNameIsRequired);
		RuleFor(x => x.PriceDisplayName).NotEmpty().WithErrorCode(ErrorCode.ProductPriceDisplayNameIsRequired);
	}
}
