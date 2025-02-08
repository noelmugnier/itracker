namespace ITracker.Core.Application;

public static class IntegrationEventMapper 
{
	public static IntegrationEvent? MapDomainEvent<T>(T domainEvent) where T : DomainEvent
	{
		return domainEvent switch
		{
			ScraperScheduledDomainEvent pageScheduled => new ScraperScheduledIntegrationEvent(pageScheduled.ScraperId.Value, pageScheduled.Cron.Value),
			ScraperUnscheduledDomainEvent pageUnscheduled => new ScraperUnscheduledIntegrationEvent(pageUnscheduled.ScraperId.Value),
			ElementParsedDomainEvent elementParsed => new ElementParsedIntegrationEvent(elementParsed.CatalogId.Value, elementParsed.ParsedProperties),
			CatalogProductFieldsUpdatedDomainEvent catalogProductFieldsUpdated => new CatalogProductFieldsUpdatedIntegrationEvent(catalogProductFieldsUpdated.CatalogId.Value, catalogProductFieldsUpdated.Fields),
			null => null,
			{ } => null
		};
	}
}
