using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ITracker.Adapters.Persistence.Configuration;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
	private readonly JsonSerializerSettings _jsonSerializerOptions = new JsonSerializerSettings
	{
		TypeNameHandling = TypeNameHandling.Objects,		
	};

	public void Configure(EntityTypeBuilder<Brand> builder)
	{
		builder.HasKey(w => w.Id);
		builder.Ignore(w => w.DomainEvents);

		builder
			.Property(w => w.Id)
			.HasConversion(v => v.Value, v => BrandId.From(v));

		builder
			.Property(w => w.Name)
			.HasConversion(v => v.Value, v => Name.From(v));

		builder
			.Property(w => w.IsDefault)
			.HasDefaultValue(false);

		builder
			.Property(w => w.CreatedOn);

		builder
			.Property(w => w.UpdatedOn);

		builder.HasIndex(t => new { t.CreatedOn, t.Id });
		builder.ToTable("brand");
	}
}
