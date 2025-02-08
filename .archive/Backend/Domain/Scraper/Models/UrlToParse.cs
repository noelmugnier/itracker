namespace ITracker.Core.Domain;

public record UrlToParse : PropertyToParse<UrlParsed>
{
	private UrlToParse() 
		: base(PropertyName.Empty(), PropertySelector.Empty(), ValueKind.Unknown, true){}

	public UrlToParse(PropertyName name, PropertySelector valueSelector, bool required = true)
		: base(name, valueSelector, ValueKind.Url, required)
	{
	}

	public override UrlParsed? Parse(string? value)
	{
		var regex = new Regex("(?i)\b(?<url>(?:[a-z][\\w-]+:(?:\\/{1,3}|[a-z0-9%])|www\\d{0,3}[.]|[a-z0-9.\\-]+[.][a-z]{2,4}\\/)(?:[^\\s()<>]+|\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\))+(?:\\(([^\\s()<>]+|(\\([^\\s()<>]+\\)))*\\)|[^\\s`!()\\[\\]{};:'\".,<>?«»“”‘’]))");

		var matches = regex.Match(value.CleanHtmlValue());
		if (matches.Success)
			return new UrlParsed(Name, new Url(matches.Groups["url"].Value));

		return Required ? null : new UrlParsed(Name, null);
	}
}
