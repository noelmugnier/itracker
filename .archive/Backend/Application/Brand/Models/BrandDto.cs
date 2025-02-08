namespace ITracker.Core.Application;

public record BrandDto
{
	public BrandDto(Guid id, string name, bool isDefault)
	{
		Id = id;
		Name = name;
		IsDefault = isDefault;
	}

	public BrandDto(Brand brand)
		: this(brand.Id.Value, brand.Name.Value, brand.IsDefault)
	{
	}

	public Guid Id { get; }
	public string Name { get; }
	public bool IsDefault { get; }
}
