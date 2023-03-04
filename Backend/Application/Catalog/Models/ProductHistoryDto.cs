namespace ITracker.Core.Application;

public class ProductHistoryDto : Dictionary<string, object?>
{
	public ProductHistoryDto(ProductHistory result)
	{
		Add("updatedOn", result.CreatedOn);

		foreach (var property in result.ModifiedFields)
			Add(property.Name.Value, property.RawValue);
	}
}
