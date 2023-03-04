namespace ITracker.Core.Domain;

public class Brand : Entity<BrandId>
{
	private Brand()
		: base(BrandId.New())
	{ }

	private Brand(Name name, bool isDefault)
		: base(BrandId.New())
	{
		Name = name;
    IsDefault = isDefault;
		CreatedOn = DateTimeOffset.UtcNow;
		UpdatedOn = DateTimeOffset.UtcNow;
	}

	public static Brand CreateDefault(Name name)
	{
		var brand = new Brand(name, true);
		brand.AddDomainEvent(new BrandCreatedDomainEvent(brand.Id));

		return brand;
	}

	public static Brand Create(Name name)
	{
		var brand = new Brand(name, false);
		brand.AddDomainEvent(new BrandCreatedDomainEvent(brand.Id));

		return brand;
	}

	public Name Name { get; set; }
  public bool IsDefault {get; set;}
	public DateTimeOffset CreatedOn { get; }
	public DateTimeOffset UpdatedOn { get; private set; }
}
