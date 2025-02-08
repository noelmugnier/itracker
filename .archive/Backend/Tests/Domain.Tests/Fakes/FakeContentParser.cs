namespace ITracker.Core.Domain.Tests;

public class FakeContentParser : IContentParser
{
    private readonly Func<string, ParsedContent>? _value;

    public FakeContentParser(Func<string, ParsedContent>? value = null)
    {
        _value = value;
    }

    public Result<ParsedContent> Parse(string content, Parser selector)
    {
        try{
            if(_value != null)
                return Result.Ok(_value(content));

            return new Result<ParsedContent>();
        }
        catch(Exception ex)
        {
            return Result.Fail<ParsedContent>(ex.Message);
        }
    }
}