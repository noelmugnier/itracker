namespace ITracker.Core.Domain
{
	public record ParsingCompletedWithErrorsDomainEvent(ParsingResultId ParsingResultId) : DomainEvent;
}