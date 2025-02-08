using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ITracker.Adapters.Persistence.Configuration;

public class ProductHistoryConfiguration : IEntityTypeConfiguration<ProductHistory>
{
	private readonly JsonSerializerSettings _jsonSerializerOptions = new JsonSerializerSettings
	{
		TypeNameHandling = TypeNameHandling.Objects,		
	};
	
	public void Configure(EntityTypeBuilder<ProductHistory> builder)
	{
		builder.HasKey(t => t.Id);
		
		builder
			.Property(e => e.CreatedOn);		
			
		builder
			.Property(p => p.ProductId)
			.HasConversion(v => v.Value, v => ProductId.From(v));

    	builder
			.OwnsOne(e => e.Price);
			
		builder
			.Property(p => p.CatalogId)
			.HasConversion(v => v.Value, v => CatalogId.From(v));		

		builder.Property(e => e.ModifiedFields)
					.HasConversion(
						v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
						v => !string.IsNullOrWhiteSpace(v) ? JsonConvert.DeserializeObject<List<PropertyParsed>>(v, _jsonSerializerOptions)! : new List<PropertyParsed>().AsReadOnly());		
				
		builder.Ignore(t => t.DomainEvents);
		
		builder.HasOne<Product>()
			.WithMany()
			.HasForeignKey(p => p.ProductId)
			.HasPrincipalKey(p => p.Id)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasIndex(t => new {t.ProductId, t.CreatedOn, t.CatalogId, t.Id });
		builder.ToTable("product_history");
	}
}