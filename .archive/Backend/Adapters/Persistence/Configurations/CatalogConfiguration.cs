using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ITracker.Adapters.Persistence.Configuration;

public class CatalogConfiguration : IEntityTypeConfiguration<Catalog>
{
	private readonly JsonSerializerSettings _jsonSerializerOptions = new JsonSerializerSettings
	{
		TypeNameHandling = TypeNameHandling.Objects,		
	};

	public void Configure(EntityTypeBuilder<Catalog> builder)
	{
		builder.HasKey(p => p.Id);
		builder.Ignore(p => p.DomainEvents);

		builder
			.Property(p => p.Id)
			.HasConversion(v => v.Value, v => CatalogId.From(v));
			
		builder
			.Property(w => w.CreatedOn);

		builder
			.Property(w => w.UpdatedOn);

		builder
			.Property(p => p.Name)
			.HasConversion(v => v.Value, v => Name.From(v));

		builder
			.Property(p => p.BrandId)
			.HasConversion(v => v.Value, v => BrandId.From(v));

		builder.OwnsOne(p => p.Schema,
			schema =>
			{					
				schema.Property(e => e.Identifier)
					.HasConversion(
						v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
						v => JsonConvert.DeserializeObject<ProductIdPropertySchema>(v, _jsonSerializerOptions)!);		

				schema.Property(e => e.Name)
					.HasConversion(
						v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
						v => JsonConvert.DeserializeObject<ProductNamePropertySchema>(v, _jsonSerializerOptions)!);	

				schema.Property(e => e.Price)
					.HasConversion(
						v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
						v => JsonConvert.DeserializeObject<ProductPricePropertySchema>(v, _jsonSerializerOptions)!);

				schema.Property(e => e.Fields)
					.HasConversion(
						v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
						v => !string.IsNullOrWhiteSpace(v) ? JsonConvert.DeserializeObject<List<ProductPropertySchema>>(v, _jsonSerializerOptions)! : new List<ProductPropertySchema>().AsReadOnly());
			});

		builder.HasOne<Brand>()
			.WithMany()
			.HasForeignKey(p => p.BrandId)
			.OnDelete(DeleteBehavior.Cascade);			

		builder.HasIndex(t => new { t.BrandId, t.CreatedOn, t.Id });
		builder.ToTable("catalog");
	}
}