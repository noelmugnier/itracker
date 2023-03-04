namespace ITracker.Core.Domain;

public interface IContentParser
{
	public Result<ParsedContent> Parse(string content, Parser parser);
}
