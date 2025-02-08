using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ITracker.Adapters.Persistence.Configuration;

public class ScraperConfiguration : IEntityTypeConfiguration<Scraper>
{
	private readonly JsonSerializerSettings _jsonSerializerOptions = new JsonSerializerSettings
	{
		TypeNameHandling = TypeNameHandling.Objects,		
	};

	public void Configure(EntityTypeBuilder<Scraper> builder)
	{
		builder.HasKey(p => p.Id);

		builder
			.Property(p => p.Id)
			.HasConversion(v => v.Value, v => ScraperId.From(v));
			
		builder
			.Property(w => w.CreatedOn);

		builder
			.Property(w => w.UpdatedOn);

		builder
			.Property(p => p.Name)
			.HasConversion(v => v.Value, v => Name.From(v));

		builder
			.Property(w => w.Uri)
			.HasConversion(v => v.Value, v => ScraperUri.From(v));
			
		builder
			.Property(p => p.CatalogId)
			.HasConversion(v => v.Value, v => CatalogId.From(v));		

		builder
			.Property(p => p.BrandId)
			.HasConversion(v => v.Value, v => BrandId.From(v));			

		builder.OwnsOne(p => p.Pagination,
			pagination =>
			{
				pagination
					.Property(pg => pg.MaxPages)
					.HasConversion(v => v.Value, v => MaxPages.From(v));

				pagination
					.Property(pg => pg.PageSize)
					.HasConversion(v => v.ToNullable(), v => PageSize.FromNullable(v));

				pagination
					.Property(pg => pg.PageNumberParameterName)
					.HasConversion(v => v.Value, v => ParameterName.From(v));

				pagination
					.Property(pg => pg.PageSizeParameterName)
					.HasConversion(v => v.ToNullable(), v => ParameterName.FromNullable(v));
			});

		builder.OwnsOne(p => p.Scheduling,
			scheduling =>
			{
				scheduling
					.Property(sc => sc.Cron)
					.HasConversion(v => v.Value, v => Cron.From(v));
			});

		builder.OwnsOne(p => p.Parser, 
			parser =>
			{
				parser
					.Property(pa => pa.ElementSelector)
					.HasConversion(v => v.Value, v => ElementSelector.From(v));

				parser.Property(e => e.Properties)
					.HasConversion(
						v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
						v => !string.IsNullOrWhiteSpace(v) ? JsonConvert.DeserializeObject<List<PropertyToParse>>(v, _jsonSerializerOptions)! : new List<PropertyToParse>().AsReadOnly());
			});

		builder.HasOne<Catalog>()
			.WithMany()
			.HasForeignKey(p => p.CatalogId)
			.OnDelete(DeleteBehavior.Cascade);
						
		builder.Ignore(p => p.DomainEvents);

		builder.HasIndex(t => new { t.BrandId, t.CatalogId, t.CreatedOn, t.Id });
		builder.ToTable("scraper");
	}
}