using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ITracker.Adapters.Persistence.Configuration;

public class OutboxIntegrationEventConfiguration : IEntityTypeConfiguration<OutboxIntegrationEvent>
{
	public void Configure(EntityTypeBuilder<OutboxIntegrationEvent> builder)
	{
		builder.HasKey(o => o.Id);
		builder.HasIndex(t => new {t.InsertedOn, t.Id});
		
		builder.ToTable("outbox_integration_event");
	}
}