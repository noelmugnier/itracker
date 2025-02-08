namespace ITracker.Core.Domain
{
	public record ParsingFailedDomainEvent(ParsingResultId ParsingResultId) : DomainEvent;
}