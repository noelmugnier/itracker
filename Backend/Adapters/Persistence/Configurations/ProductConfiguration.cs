using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ITracker.Adapters.Persistence.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
	private readonly JsonSerializerSettings _jsonSerializerOptions = new JsonSerializerSettings
	{
		TypeNameHandling = TypeNameHandling.Objects,
	};

	public void Configure(EntityTypeBuilder<Product> builder)
	{
		builder.HasKey(t => new { t.CatalogId, t.Id });

		builder
			.Property(e => e.Id)
			.HasConversion(v => v.Value, v => ProductId.From(v));

		builder
			.Property(e => e.Name)
			.HasConversion(v => v.Value, v => Name.From(v));

    	builder
			.OwnsOne(e => e.Price);

		builder
			.Property(p => p.CatalogId)
			.HasConversion(v => v.Value, v => CatalogId.From(v));

		builder
			.Property(w => w.CreatedOn);

		builder
			.Property(w => w.UpdatedOn);

		builder.Property(e => e.Fields)
					.HasConversion(
						v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
						v => !string.IsNullOrWhiteSpace(v) ? JsonConvert.DeserializeObject<List<PropertyParsed>>(v, _jsonSerializerOptions)! : new List<PropertyParsed>().AsReadOnly());

		builder.Ignore(t => t.DomainEvents);
		
		builder.HasOne<Catalog>()
			.WithMany()
			.HasForeignKey(p => p.CatalogId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(t => new { t.CatalogId, t.CreatedOn, t.Id });
		builder.ToTable("product");
	}
}
