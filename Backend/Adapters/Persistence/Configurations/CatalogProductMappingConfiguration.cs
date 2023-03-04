using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITracker.Adapters.Persistence.Configuration;

public class CatalogProductMappingConfiguration : IEntityTypeConfiguration<CatalogProductMapping>
{
	public void Configure(EntityTypeBuilder<CatalogProductMapping> builder)
	{
		builder.HasKey(p => new { p.SourceCatalogId, p.TargetCatalogId, p.SourceProductId, p.TargetProductId });
		builder.Ignore(p => p.DomainEvents);

		builder
			.Property(p => p.SourceCatalogId)
			.HasConversion(v => v.Value, v => CatalogId.From(v));

		builder
			.Property(p => p.TargetCatalogId)
			.HasConversion(v => v.Value, v => CatalogId.From(v));

		builder
			.Property(p => p.SourceProductId)
			.HasConversion(v => v.Value, v => ProductId.From(v));

		builder
			.Property(p => p.TargetProductId)
			.HasConversion(v => v.Value, v => ProductId.From(v));		

		builder.ToTable("catalog_product_mapping");
	}
}