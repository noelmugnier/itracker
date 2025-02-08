namespace ITracker.Core.Application;

public class ProductDto : Dictionary<string, object?>
{
	public ProductDto(Product result)
	{
		Add(PropertyName.Identifier.Value, result.Id);
		Add(PropertyName.DisplayName.Value, result.Name);
		Add("createdOn", result.CreatedOn);
		Add("updatedOn", result.UpdatedOn);
		Add("catalogId", result.CatalogId.Value);

		foreach (var property in result.Fields)
			Add(property.Name.Value, property.RawValue);
	}	
}