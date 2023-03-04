using ITracker.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace ITracker.Adapters.Persistence.Configuration;

public class ParsingResultConfiguration : IEntityTypeConfiguration<ParsingResult>
{
	private readonly JsonSerializerSettings _jsonSerializerOptions = new JsonSerializerSettings
	{
		TypeNameHandling = TypeNameHandling.Objects,		
	};

	public void Configure(EntityTypeBuilder<ParsingResult> builder)
	{
		builder.HasKey(t => t.Id);
		
		builder
			.Property(e => e.Id)
			.HasConversion(v => v.Value, v => ParsingResultId.From(v));

		builder
			.Property(e => e.ScraperId)
			.HasConversion(v => v.Value, v => ScraperId.From(v));
			
		builder
			.Property(w => w.StartedOn);

		builder
			.Property(w => w.CreatedOn);

		builder
			.Property(w => w.EndedOn);

		builder
			.Property(w => w.Status);

		builder.OwnsMany(t => t.Elements, element => {
			element.HasKey(e => e.Id);
				
			element.Property(e => e.Properties)
				.HasConversion(
					v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
					v => !string.IsNullOrWhiteSpace(v) ? JsonConvert.DeserializeObject<List<PropertyParsed>>(v, _jsonSerializerOptions)! : new List<PropertyParsed>().AsReadOnly());

			element.ToTable("parsing_result_element");
		});
		
		builder.Property(w => w.Errors)
			.HasConversion(
				v => JsonConvert.SerializeObject(v, _jsonSerializerOptions),
				v => !string.IsNullOrWhiteSpace(v) ? JsonConvert.DeserializeObject<List<ParsingError>>(v, _jsonSerializerOptions)! : new List<ParsingError>().AsReadOnly());
		
		builder.HasOne<Scraper>()
			.WithMany()
			.HasForeignKey(p => p.ScraperId)
			.OnDelete(DeleteBehavior.Cascade);
		
		builder.Ignore(t => t.DomainEvents);

		builder.HasIndex(t => new { t.ScraperId, t.CreatedOn, t.Id });
		builder.ToTable("parsing_result");
	}
}