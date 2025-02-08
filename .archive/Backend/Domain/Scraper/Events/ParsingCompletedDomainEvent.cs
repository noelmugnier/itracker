namespace ITracker.Core.Domain
{
	public record ParsingCompletedDomainEvent(ParsingResultId ParsingResultId) : DomainEvent;
}